const generateCodeFromStructure = ( { name, classes, properties, isDictionaryExtension }, generatedClasses = {} ) => {
    const output = [];
    for (let classDefinition of classes ) {
        if (generatedClasses[classDefinition.name]) {
            continue;
        }
        generatedClasses[classDefinition.name] = true;
        output.push( generateCodeFromStructure( classDefinition, generatedClasses ) );
        output.push( '' );
    }

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
    return output.join('\n');
}

exports.generateCodeFromStructure = generateCodeFromStructure;