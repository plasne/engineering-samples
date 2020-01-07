import { inject, injectable } from 'tsyringe';
import { IDatabase } from './IDatabase';

@injectable()
export class Processor {
    constructor(@inject('IDatabase') private database: IDatabase) {}

    public process() {
        return this.database.write();
    }
}
