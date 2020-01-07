export abstract class Logger {
    public abstract info(msg: string): void;

    public abstract debug(msg: string): void;

    public abstract error(msg: string): void;

    public abstract warn(msg: string): void;

    public abstract trace(msg: string): void;

    public get LOG_LEVEL() {
        return process.env.LOG_LEVEL || 'info';
    }

    public get DISABLE_COLORS() {
        return process.env.DISABLE_COLORS || false;
    }
}
