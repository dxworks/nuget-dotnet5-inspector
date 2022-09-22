#! /usr/bin/env node
'use strict'
const {runNugetInspector} = require('./index');

(async () => {
    await runNugetInspector();
})();