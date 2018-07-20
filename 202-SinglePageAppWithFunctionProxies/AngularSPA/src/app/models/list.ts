import { Task } from "./task";

export interface List {
    Id: string;
    Name: string;
    IsVirtual: boolean;
    Tasks: Array<Task>;
}
