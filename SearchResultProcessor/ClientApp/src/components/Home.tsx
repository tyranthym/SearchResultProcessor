import React from "react";
import { PageHeader, Form } from "antd";
import Aux from "../hoc/Auxiliary";
import { SearchForm} from "./SearchForm";
import SpaceSeparator from "../utilities/SpaceSeparator";

const SearchFormWrapped = Form.create({ name: "search_form" })(SearchForm);

const Home = () => {
  return (
    <Aux>
      <PageHeader
        style={{ border: "1px solid rgb(235, 237, 240)" }}        
        title="Simple Web Scraper"
        subTitle="chrome webdriver simulation"
      />
      <SpaceSeparator />
      <SpaceSeparator />
      <SpaceSeparator />
      <SearchFormWrapped />
    </Aux>
  );
};

export default Home;
