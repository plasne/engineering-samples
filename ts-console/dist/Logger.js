"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Logger {
    get LOG_LEVEL() {
        return process.env.LOG_LEVEL || 'info';
    }
    get DISABLE_COLORS() {
        return process.env.DISABLE_COLORS || false;
    }
}
exports.Logger = Logger;
//# sourceMappingURL=Logger.js.map