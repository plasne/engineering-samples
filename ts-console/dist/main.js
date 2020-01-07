"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
// imports
const appInsights = require("applicationinsights");
const dotenv_1 = require("dotenv");
const findup_sync_1 = __importDefault(require("findup-sync"));
require("reflect-metadata");
const tsyringe_1 = require("tsyringe");
const ConsoleLogger_1 = require("./ConsoleLogger");
const Processor_1 = require("./Processor");
const RealDatabase_1 = require("./RealDatabase");
// load the .env by traversing up the path
const env = findup_sync_1.default('.env');
if (env)
    dotenv_1.config({ path: env });
// start AppInsights with defaults
appInsights.setup().start();
// register for dependency injection (can also useClass for transients)
const logger = new ConsoleLogger_1.ConsoleLogger();
tsyringe_1.container.register('Logger', { useValue: logger });
const database = tsyringe_1.container.resolve(RealDatabase_1.RealDatabase);
tsyringe_1.container.register('IDatabase', { useValue: database });
// process
const start = new Date();
const processor = tsyringe_1.container.resolve(Processor_1.Processor);
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
//# sourceMappingURL=main.js.map