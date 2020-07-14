const generators = require("./generators");

const generateCodeFromStructure = ( { name, classes, properties, isDictionaryExtension }, serializer = serializeClassAsCS, generatedClasses = {} ) => {
    const output = [];
    for (let classDefinition of classes ) {
        if (generatedClasses[classDefinition.name]) {
            continue;
        }
        generatedClasses[classDefinition.name] = true;
        output.push( generateCodeFromStructure( classDefinition, serializer, generatedClasses ) );
        output.push( '' );
    }
    output.push( ...serializer({ name, properties, isDictionaryExtension }) );
    return output.join('\n');
}

function serializerFactory( target = "cs" ) {
    const serializer = generators['serializeClassAs' + target.toUpperCase()];
    if (!serializer) {
        throw `Target: ${target} wasn't found in generators`;
    }
    return serializer;
}

exports.generateCodeFromStructure = (structure, target = "CS") => generateCodeFromStructure( structure, serializerFactory( target ) );