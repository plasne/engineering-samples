import { inject, injectable } from 'tsyringe';
import { IDatabase } from './IDatabase';
import { Logger } from './Logger';

@injectable()
export class RealDatabase implements IDatabase {
    constructor(@inject('Logger') private logger: Logger) {}

    public write() {
        this.logger.debug('wrote to real database.');
        return '43fbf4df-bc55-4977-b219-7d671639b954';
    }
}
