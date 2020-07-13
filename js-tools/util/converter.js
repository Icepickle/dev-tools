const fs = require('fs');
const { readFromJson } = require('./pocoReader');
const { generateCodeFromStructure } = require('./pocoWriter');
const { parseOptions } = require('./util');

const options = [
    { shortcut: 'i', isDefault: true, required: true, arg: 'input', property: 'inputFile' },
    { shortcut: 'n', arg: 'name', property: 'className', defaultValue: 'rootObject' },
    { shortcut: 'o', arg: 'output', property: 'outputFile' },
    { shortcut: 'ns', arg: 'namespace', property: 'namespace' }
];

const programOptions = parseOptions( process.argv, options );

fs.readFile( programOptions.inputFile, 'utf8', ( err, data ) => {
    if (err) {
        throw err;
    }
    const structure = readFromJson( JSON.parse( data ), programOptions.className )
    let result = generateCodeFromStructure( structure );

    if (programOptions.namespace) {
        result = `using NewtonSoft.Json;\nusing Newtonsoft.Json.Linq;\n\nnamespace ${programOptions.namespace} {\n${result}\n}`;
    }

    if ( programOptions.outputFile ) {
        fs.writeFile( programOptions.outputFile, result, 'utf8', () => {
            console.log( `Created ${programOptions.outputFile}` );
        } );
    } else {
        console.log( result );
    }
} );