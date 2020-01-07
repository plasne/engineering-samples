// imports
import appInsights = require('applicationinsights');
import { config } from 'dotenv';
import findup from 'findup-sync';
import 'reflect-metadata';
import { container } from 'tsyringe';
import { ConsoleLogger } from './ConsoleLogger';
import { Processor } from './Processor';
import { RealDatabase } from './RealDatabase';

// load the .env by traversing up the path
const env = findup('.env');
if (env) config({ path: env });

// start AppInsights with defaults
appInsights.setup().start();

// register for dependency injection (can also useClass for transients)
const logger = new ConsoleLogger();
container.register('Logger', { useValue: logger });
const database = container.resolve(RealDatabase);
container.register('IDatabase', { useValue: database });

// process
const start = new Date();
const processor = container.resolve(Processor);
const guid = processor.process();
logger.info(`written as "${guid}".`);

// track request in app insights
logger.info(`waiting on telemetry to flush...`);
appInsights.defaultClient.trackRequest({
    duration: start.valueOf() - new Date().valueOf(),
    name: 'process()',
    resultCode: 200,
    success: true,
    url: 'http://process'
});
