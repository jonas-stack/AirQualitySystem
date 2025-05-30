import { BaseDto } from 'ws-request-hook';
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.3.0.0 (NJsonSchema v11.2.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

export class AiClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    subscribe(authorization: string | undefined, message: MessageFromClient): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Ai/chat";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(message);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "authorization": authorization !== undefined && authorization !== null ? "" + authorization : "",
                "Content-Type": "application/json",
                "Accept": "application/octet-stream"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processSubscribe(_response);
        });
    }

    protected processSubscribe(response: Response): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            let fileNameMatch = contentDisposition ? /filename\*=(?:(\\?['"])(.*?)\1|(?:[^\s]+'.*?')?([^;\n]*))/g.exec(contentDisposition) : undefined;
            let fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[3] || fileNameMatch[2] : undefined;
            if (fileName) {
                fileName = decodeURIComponent(fileName);
            } else {
                fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
                fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            }
            return response.blob().then(blob => { return { fileName: fileName, data: blob, status: status, headers: _headers }; });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<FileResponse>(null as any);
    }

    historicalDataAnalysis(authorization: string | undefined, message: MessageFromClient): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Ai/historical_data_analysis";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(message);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "authorization": authorization !== undefined && authorization !== null ? "" + authorization : "",
                "Content-Type": "application/json",
                "Accept": "application/octet-stream"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processHistoricalDataAnalysis(_response);
        });
    }

    protected processHistoricalDataAnalysis(response: Response): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            let fileNameMatch = contentDisposition ? /filename\*=(?:(\\?['"])(.*?)\1|(?:[^\s]+'.*?')?([^;\n]*))/g.exec(contentDisposition) : undefined;
            let fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[3] || fileNameMatch[2] : undefined;
            if (fileName) {
                fileName = decodeURIComponent(fileName);
            } else {
                fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
                fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            }
            return response.blob().then(blob => { return { fileName: fileName, data: blob, status: status, headers: _headers }; });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<FileResponse>(null as any);
    }
}

export class SubscriptionClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    subscribe(dto: ChangeSubscriptionDto): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Subscription/Subscribe";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(dto);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/octet-stream"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processSubscribe(_response);
        });
    }

    protected processSubscribe(response: Response): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            let fileNameMatch = contentDisposition ? /filename\*=(?:(\\?['"])(.*?)\1|(?:[^\s]+'.*?')?([^;\n]*))/g.exec(contentDisposition) : undefined;
            let fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[3] || fileNameMatch[2] : undefined;
            if (fileName) {
                fileName = decodeURIComponent(fileName);
            } else {
                fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
                fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            }
            return response.blob().then(blob => { return { fileName: fileName, data: blob, status: status, headers: _headers }; });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<FileResponse>(null as any);
    }

    unsubscribe(dto: ChangeSubscriptionDto): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Subscription/Unsubscribe";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(dto);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/octet-stream"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processUnsubscribe(_response);
        });
    }

    protected processUnsubscribe(response: Response): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            let fileNameMatch = contentDisposition ? /filename\*=(?:(\\?['"])(.*?)\1|(?:[^\s]+'.*?')?([^;\n]*))/g.exec(contentDisposition) : undefined;
            let fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[3] || fileNameMatch[2] : undefined;
            if (fileName) {
                fileName = decodeURIComponent(fileName);
            } else {
                fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
                fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            }
            return response.blob().then(blob => { return { fileName: fileName, data: blob, status: status, headers: _headers }; });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<FileResponse>(null as any);
    }
}

export interface MessageFromClient {
    message?: string;
}

export interface ChangeSubscriptionDto {
    clientId?: string;
    topicIds?: string[];
}



export interface WebsocketMessage_1 extends BaseDto {
    topic?: string;
    data?: T;
}

export interface T {
}

export interface ApplicationBaseDto {
    eventType?: string;
    topic?: string;
}

export interface DeviceIntervalUpdateDto extends BaseDto {
    deviceId?: string;
    interval?: number;
}

export interface LiveAiFeedbackDto extends BaseDto {
    aiAdvice?: string;
}

export interface ServerSendsErrorMessage extends BaseDto {
    message?: string;
}

export interface ClientRequestAiLiveData extends BaseDto {
}

export interface RequestAirQualityData extends BaseDto {
    timePeriod?: TimePeriod;
    data?: string[];
}

export enum TimePeriod {
    Hourly = "Hourly",
    Daily = "Daily",
    Weekly = "Weekly",
    Monthly = "Monthly",
}

export interface ServerResponseAirQualityData extends BaseDto {
    requestedData?: string[];
    timePeriod?: TimePeriod;
    data?: { [key: string]: any; }[];
}

export interface ClientRequestDeviceHistory extends BaseDto {
    deviceId?: string;
    pageNumber?: number;
    pageSize?: number;
}

export interface ServerResponseDeviceHistory extends BaseDto {
    connectionData?: PagedResultOfDeviceConnectionHistoryDto;
}

export interface PagedResultOfDeviceConnectionHistoryDto {
    items?: DeviceConnectionHistoryDto[];
    pageNumber?: number;
    pageSize?: number;
    totalCount?: number;
    totalPages?: number;
}

export interface DeviceConnectionHistoryDto {
    id?: number;
    deviceId?: string;
    isConnected?: boolean;
    lastSeen?: number;
    duration?: number | undefined;
}

export interface ClientRequestDeviceStatus extends BaseDto {
}

export interface ServerResponseDeviceStatus extends BaseDto {
    deviceStatus?: DeviceDto;
}

export interface DeviceDto {
    device_id?: string;
    DeviceName?: string;
    LastSeen?: number;
    IsConnected?: boolean;
    updateInterval?: number;
}

export interface ClientRequestDeviceStats extends BaseDto {
}

export interface ServerResponseDeviceStats extends BaseDto {
    stats?: DeviceStatsDto;
}

export interface DeviceStatsDto {
    allTimeMeasurements?: number;
    connectedDevices?: number;
    disconnectionsLast24Hours?: number;
}

export interface ClientRequestSensorData extends BaseDto {
    sensorId?: string;
    pageNumber?: number;
    pageSize?: number;
}

export interface ServerResponseSensorData extends BaseDto {
    sensorData?: PagedResultOfSensorDataDto;
}

export interface PagedResultOfSensorDataDto {
    items?: SensorDataDto[];
    pageNumber?: number;
    pageSize?: number;
    totalCount?: number;
    totalPages?: number;
}

export interface SensorDataDto {
    temperature?: number;
    humidity?: number;
    air_quality?: number;
    pm25?: number;
    device_id?: string;
    timestamp?: number;
}

export interface ServerResponseDeviceUpdateInterval extends BaseDto {
    success?: boolean;
}

export interface ClientRequestDeviceList extends BaseDto {
}

export interface ServerResponseDeviceList extends BaseDto {
    deviceList?: DeviceDto[];
}

export interface Ping extends BaseDto {
}

export interface Pong extends BaseDto {
}

/** Websocket Topic Enums */
export enum WebsocketTopics {
    Dashboard = "Dashboard",
    Ai = "Ai",
    DeviceStatus = "DeviceStatus",
    DeviceData = "DeviceData",
    Device = "Device",
    Graph = "Graph",
    GraphTotalMeasurements = "GraphTotalMeasurements",
}

/** Websocket event types (constants + BaseDto inheritors) */
export enum WebsocketEvents {
    AllDeviceStatus = "AllDeviceStatus",
    BroadcastDeviceConnectionUpdate = "BroadcastDeviceConnectionUpdate",
    BroadcastDeviceSensorDataUpdate = "BroadcastDeviceSensorDataUpdate",
    ClientRequestAiLiveData = "ClientRequestAiLiveData",
    ClientRequestDeviceHistory = "ClientRequestDeviceHistory",
    ClientRequestDeviceList = "ClientRequestDeviceList",
    ClientRequestDeviceStats = "ClientRequestDeviceStats",
    ClientRequestDeviceStatus = "ClientRequestDeviceStatus",
    ClientRequestSensorData = "ClientRequestSensorData",
    DeviceIntervalUpdateDto = "DeviceIntervalUpdateDto",
    DeviceUpdateStatus = "DeviceUpdateStatus",
    GraphGetMeasurement = "GraphGetMeasurement",
    GraphTemperatureUpdate = "GraphTemperatureUpdate",
    GraphTotalMeasurement = "GraphTotalMeasurement",
    LiveAiFeedbackDto = "LiveAiFeedbackDto",
    Ping = "Ping",
    Pong = "Pong",
    RequestAirQualityData = "RequestAirQualityData",
    ServerResponseAirQualityData = "ServerResponseAirQualityData",
    ServerResponseDeviceHistory = "ServerResponseDeviceHistory",
    ServerResponseDeviceList = "ServerResponseDeviceList",
    ServerResponseDeviceStats = "ServerResponseDeviceStats",
    ServerResponseDeviceStatus = "ServerResponseDeviceStatus",
    ServerResponseDeviceUpdateInterval = "ServerResponseDeviceUpdateInterval",
    ServerResponseSensorData = "ServerResponseSensorData",
    ServerSendsErrorMessage = "ServerSendsErrorMessage",
    WebsocketMessage_1 = "WebsocketMessage`1",
}

export interface FileResponse {
    data: Blob;
    status: number;
    fileName?: string;
    headers?: { [name: string]: any };
}

export class ApiException extends Error {
    override message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
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

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}