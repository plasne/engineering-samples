import { expect } from 'chai';
import { config } from 'dotenv';
import findup from 'findup-sync';
import { suite, test } from 'mocha-typescript';
import 'reflect-metadata';
import { container } from 'tsyringe';
import * as TypeMoq from 'typemoq';
import { isUuid } from 'uuidv4';
import { IDatabase } from './IDatabase';
import { Processor } from './Processor';

@suite('Process Tests')
class ProcessTests {
    private static before() {
        // load the .env by traversing up the path
        const env = findup('.env');
        /* istanbul ignore next ; an .env file is not required */
        if (env) config({ path: env });

        // create a mock database dependency
        const mock = TypeMoq.Mock.ofType<IDatabase>();
        mock.setup(x => x.write()).returns(
            () => 'b8c4c915-7e1f-4274-83ac-345c812ab807'
        );

        // register for dependency injection
        container.register('IDatabase', {
            useValue: mock.object
        });
    }

    @test('can process') private canProcess() {
        const processor = container.resolve(Processor);
        const guid = processor.process();
        // tslint:disable-next-line: no-unused-expression
        expect(isUuid(guid), 'process should return a GUID').to.be.true;
    }
}
