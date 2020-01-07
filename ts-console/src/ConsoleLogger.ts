import * as winston from 'winston';
import { Logger } from './Logger';

export class ConsoleLogger extends Logger {
    private log: winston.Logger;

    constructor() {
        super();
        const foreground: {
            [index: string]: string;
        } = {
            debug: '\u001b[37m', // white on black
            error: '\u001b[30m', // black on red
            info: '\u001b[32m', // green on black
            silly: '\u001b[37m', // white on black
            verbose: '\u001b[37m', // white on black
            warn: '\u001b[33m' // yellow on black
        };
        const background: {
            [index: string]: string;
        } = {
            debug: '\u001b[40m', // white on black
            error: '\u001b[41m', // black on red
            info: '\u001b[40m', // green on black
            silly: '\u001b[40m', // white on black
            verbose: '\u001b[40m', // white on black
            warn: '\u001b[40m' // yellow on black
        };
        const short: {
            [index: string]: string;
        } = {
            debug: 'dbug',
            error: 'fail',
            info: 'info',
            silly: 'trce',
            verbose: 'trce',
            warn: 'warn'
        };
        const transport = new winston.transports.Console({
            format: winston.format.combine(
                winston.format.timestamp(),
                winston.format.printf(event => {
                    if (super.DISABLE_COLORS) {
                        return `${short[event.level]} ${event.timestamp} ${
                            event.message
                        }`;
                    } else {
                        return `${foreground[event.level] || ''}${background[
                            event.level
                        ] || ''}${short[event.level]}\u001b[0m ${
                            event.timestamp
                        } ${event.message}`;
                    }
                })
            )
        });
        this.log = winston.createLogger({
            level: super.LOG_LEVEL,
            transports: [transport]
        });
    }

    public info(msg: string) {
        this.log.info(msg);
    }

    public debug(msg: string) {
        this.log.debug(msg);
    }

    public error(msg: string) {
        this.log.error(msg);
    }

    public warn(msg: string) {
        this.log.warn(msg);
    }

    public trace(msg: string) {
        this.log.verbose(msg);
    }
}
