export interface IDatabase {
    write(): string; // note: returns ID of the written record
}
