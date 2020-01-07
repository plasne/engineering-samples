"use strict";
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const winston = __importStar(require("winston"));
const Logger_1 = require("./Logger");
class ConsoleLogger extends Logger_1.Logger {
    constructor() {
        super();
        const foreground = {
            debug: '\u001b[37m',
            error: '\u001b[30m',
            info: '\u001b[32m',
            silly: '\u001b[37m',
            verbose: '\u001b[37m',
            warn: '\u001b[33m' // yellow on black
        };
        const background = {
            debug: '\u001b[40m',
            error: '\u001b[41m',
            info: '\u001b[40m',
            silly: '\u001b[40m',
            verbose: '\u001b[40m',
            warn: '\u001b[40m' // yellow on black
        };
        const short = {
            debug: 'dbug',
            error: 'fail',
            info: 'info',
            silly: 'trce',
            verbose: 'trce',
            warn: 'warn'
        };
        const transport = new winston.transports.Console({
            format: winston.format.combine(winston.format.timestamp(), winston.format.printf(event => {
                if (super.DISABLE_COLORS) {
                    return `${short[event.level]} ${event.timestamp} ${event.message}`;
                }
                else {
                    return `${foreground[event.level] || ''}${background[event.level] || ''}${short[event.level]}\u001b[0m ${event.timestamp} ${event.message}`;
                }
            }))
        });
        this.log = winston.createLogger({
            level: super.LOG_LEVEL,
            transports: [transport]
        });
    }
    info(msg) {
        this.log.info(msg);
    }
    debug(msg) {
        this.log.debug(msg);
    }
    error(msg) {
        this.log.error(msg);
    }
    warn(msg) {
        this.log.warn(msg);
    }
    trace(msg) {
        this.log.verbose(msg);
    }
}
exports.ConsoleLogger = ConsoleLogger;
//# sourceMappingURL=ConsoleLogger.js.map