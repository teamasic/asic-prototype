export default class ApiResponse {
    success: boolean = true;
    errors: any[] = [];
    data: any;
    statusCode: number = 0;
}