"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const chai_1 = require("chai");
const dotenv_1 = require("dotenv");
const findup_sync_1 = __importDefault(require("findup-sync"));
const mocha_typescript_1 = require("mocha-typescript");
require("reflect-metadata");
const tsyringe_1 = require("tsyringe");
const TypeMoq = __importStar(require("typemoq"));
const uuidv4_1 = require("uuidv4");
const Processor_1 = require("./Processor");
let ProcessTests = class ProcessTests {
    static before() {
        // load the .env by traversing up the path
        const env = findup_sync_1.default('.env');
        /* istanbul ignore next ; an .env file is not required */
        if (env)
            dotenv_1.config({ path: env });
        // create a mock database dependency
        const mock = TypeMoq.Mock.ofType();
        mock.setup(x => x.write()).returns(() => 'b8c4c915-7e1f-4274-83ac-345c812ab807');
        // register for dependency injection
        tsyringe_1.container.register('IDatabase', {
            useValue: mock.object
        });
    }
    canProcess() {
        const processor = tsyringe_1.container.resolve(Processor_1.Processor);
        const guid = processor.process();
        // tslint:disable-next-line: no-unused-expression
        chai_1.expect(uuidv4_1.isUuid(guid), 'process should return a GUID').to.be.true;
    }
};
__decorate([
    mocha_typescript_1.test('can process'),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", []),
    __metadata("design:returntype", void 0)
], ProcessTests.prototype, "canProcess", null);
ProcessTests = __decorate([
    mocha_typescript_1.suite('Process Tests')
], ProcessTests);
//# sourceMappingURL=test.js.map