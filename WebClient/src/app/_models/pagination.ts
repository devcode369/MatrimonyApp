export interface Pagination{

    currentPage:number;
    itemPerPage:number;
    totalItems:number;
    totalPages:number;

}

export class Paginationresult<T>{
    result?:T ;
    pagination?:Pagination;
}