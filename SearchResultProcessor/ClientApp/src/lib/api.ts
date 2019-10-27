//error
export interface ValidationProblemDetails {
    readonly errors?: { [key: string]: string[] } | undefined;
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;
}

export interface ErrorResponse {
    type?: string | undefined;
    errors?: string[] | undefined;
    message?: string | undefined;
}

export type Api4xxErrorResponses =
    | ValidationProblemDetails
    | ErrorResponse;
    

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any };
    result: any;

    constructor(
        message: string,
        status: number,
        response: string,
        headers: { [key: string]: any },
        result: any
    ) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

//request
export interface ScrapeRequest {
    keywords?: string | undefined;
    resultNum?: number | undefined;
    matchingUrl?: string | undefined;
}