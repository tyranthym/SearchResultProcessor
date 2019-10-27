import React, { useRef, useCallback, useState, useEffect } from "react";
import { Select, Input, Button, Form, Icon, message, Descriptions } from "antd";
import {
  Api4xxErrorResponses,
  ValidationProblemDetails,
  ErrorResponse
} from "../lib/api";
import axios from "axios";
import * as _ from "lodash";
import SpaceSeparator from "../utilities/SpaceSeparator";

function isValidationProblemDetailsOrErrorResponse(
  obj: any
): obj is ValidationProblemDetails | ErrorResponse {
  return obj && "errors" in obj;
}

function isValidationProblemDetails(
  obj: Api4xxErrorResponses
): obj is ValidationProblemDetails {
  return obj && "errors" in obj && !_.isArray(obj.errors);
}

function isErrorResponse(obj: Api4xxErrorResponses): obj is ErrorResponse {
  return obj && "errors" in obj && _.isArray(obj.errors);
}

function mapProblemsToFieldState(
  problems: ValidationProblemDetails,
  getFieldValue: Function,
  setFields: Function
) {
  let values = {};
  for (var [key, value] of Object.entries(problems.errors)) {
    values[key] = {
      value: getFieldValue(key),
      errors: value.map(re => new Error(re))
    };
  }
  setFields(values);
}

function hasErrors(fieldsError: { [x: string]: unknown }) {
  return Object.keys(fieldsError).some(field => fieldsError[field]);
}

export function SearchForm({ form }) {
  const formRef = useRef(form);

  const [requestState, setRequestState] = useState({
    data: undefined,
    fetching: false,
    ok: undefined,
    problems: undefined,
    error: undefined
  });

  const [resultState, setResultState] = useState({
    positions: null,
    showResult: false
  });

  function setInitialState() {
    setRequestState({
      fetching: false,
      error: undefined,
      ok: undefined,
      problems: undefined,
      data: undefined
    });
  }

  function setStateToFetching() {
    setRequestState({
      data: undefined,
      fetching: true,
      ok: undefined,
      problems: undefined,
      error: undefined
    });
  }

  function setSuccessfulResponseState(data: any) {
    setRequestState({
      data: data,
      fetching: false,
      ok: true,
      problems: undefined,
      error: undefined
    });
  }

  function setValidationProblemState(problems: Api4xxErrorResponses) {
    setRequestState({
      data: undefined,
      fetching: false,
      ok: false,
      problems: problems,
      error: undefined
    });
  }

  function setErrorState(error: any) {
    setRequestState({
      data: undefined,
      fetching: false,
      ok: false,
      problems: undefined,
      error: error
    });
  }

  useEffect(() => {
    console.log(requestState);
    if (requestState.ok) {
      formRef.current.resetFields();
      console.log(requestState.data.data.message);
      if (requestState && requestState.data && requestState.data.data) {
        setResultState({
          positions: requestState.data.data.matchingPositions,
          showResult: true
        });
        message.success(requestState.data.data.message);
      } else {
        message.success("Process Successsfully!");
      }
    } else if (requestState.problems) {
      if (isValidationProblemDetails(requestState.problems)) {
        mapProblemsToFieldState(
          requestState.problems,
          formRef.current.getFieldValue,
          formRef.current.setFields
        );
      } else if (isErrorResponse(requestState.problems)) {
        message.error(requestState.problems.errors[0]);
      }
    }
  }, [requestState]);

  // const valid = !hasErrors(formRef.current.getFieldsError());
  const submit = useCallback(e => {
    e.preventDefault();
    const url = "http://localhost:3000/api/scraping";
    formRef.current.validateFields(async (err: any, values: any) => {
      console.log(values);
      if (!err) {
        setStateToFetching();
        try {
          const response = await axios.get(url, { params: values });
          setSuccessfulResponseState(response);
          console.log("setSuccessfulState");
        } catch (ex) {
          const data = ex && ex.response && ex.response.data;
          //400 or 500
          if (isValidationProblemDetailsOrErrorResponse(data)) {
            // <--
            debugger;
            setValidationProblemState(data); // <-
            console.log("setValidationProblemState");
          } else {
            debugger;
            setErrorState(ex);
            console.log("setValidationProblemState");
            console.error(ex);
          }
        }
      }
    });
  }, []);

  const { getFieldDecorator, getFieldsError } = form;

  const valid = !hasErrors(formRef.current.getFieldsError());

  return (
    <>
      <Form
        layout="horizontal"
        onSubmit={submit}
        labelCol={{ xs: { span: 4, push: 4 } }}
        wrapperCol={{ xs: { span: 8, push: 4 } }}
      >
        <Form.Item label="Searching Keywords">
          {getFieldDecorator("keywords", {
            rules: [{ required: true }]
          })(
            <Input
              style={{ width: 320 }}
              placeholder="E.g. online title search"
            />
          )}
        </Form.Item>
        <Form.Item label="Matching Keywords">
          {getFieldDecorator("matchingKeywords", {
            rules: [{ required: true }]
          })(
            <Input
              style={{ width: 320 }}
              placeholder="E.g. www.infotrack.com.au"
            />
          )}
        </Form.Item>
        <Form.Item label="Search Number">
          {getFieldDecorator("resultNum", {
            rules: [{ required: true }]
          })(
            <Select style={{ width: 160 }} placeholder="Select a Number">
              <Select.Option key="searchNum1" value="10">
                10
              </Select.Option>
              <Select.Option key="searchNum2" value="20">
                20
              </Select.Option>
              <Select.Option key="searchNum3" value="50">
                50
              </Select.Option>
              <Select.Option key="searchNum4" value="100">
                100
              </Select.Option>
            </Select>
          )}
        </Form.Item>
        <SpaceSeparator />
        <Form.Item wrapperCol={{ xs: { span: 8, push: 12 } }}>
          <Button
            type="primary"
            size="large"
            htmlType="submit"
            disabled={!valid || requestState.fetching}
          >
            Scrape
          </Button>
        </Form.Item>
      </Form>
      {resultState.showResult === true ? (
        <>
          <SpaceSeparator />
          <SpaceSeparator />
          <SpaceSeparator />
          <Descriptions title="Matching Result" layout="vertical" bordered>
            <Descriptions.Item label="Matching Position">
              {resultState.positions}
            </Descriptions.Item>
          </Descriptions>
        </>
      ) : null}
    </>
  );
}
