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

const serializeClassAsCS = ({ name, properties, isDictionaryExtension }) => {
    const output = [];
    output.push( `  public sealed class ${name} {` );
    
    if (isDictionaryExtension) {
        output.push( `    [JsonExtensionData]` );
        output.push( `    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();` );
    }

    for (let propDefinition of properties ) {
        output.push( `    [JsonProperty( "${propDefinition.propertyName}" )]` );
        output.push( `    public ${propDefinition.type} ${propDefinition.name} { get; set; }` );
    }

    output.push( `  }` );
    return output;
}

const serializeClassAsTS = ({ name, properties, isDictionaryExtension }) => {
    const output = [];
    output.push( `  export interface ${name} {` );
    
    if (isDictionaryExtension) {
        output.push( `    [x: string]: any;` );
    }

    for (let propDefinition of properties ) {
        output.push( `    ${propDefinition.propertyName}: ${propDefinition.type};` );
    }

    output.push( `  }` );
    return output;
}

function serializerFactory( target = "cs" ) {
    switch (target.toUpperCase()) {
        case "TS":
            return serializeClassAsTS;
        default:
            return serializeClassAsCS;
    }
}

exports.generateCodeFromStructure = (structure, target = "CS") => generateCodeFromStructure( structure, serializerFactory( target ) );