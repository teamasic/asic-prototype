export default interface PaginatedList<T> {
    list: T[];
    page: number;
    total: number;
    totalPage: number;
}