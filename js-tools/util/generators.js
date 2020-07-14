const { typeNames } = require("./pocoReader");

const serializeTypeToCS = typeDescription => {
  if (typeDescription.isArray && typeDescription.type === typeNames.object) {
      return `JArray`
  }
  const nullableChar = typeDescription.type === typeNames.decimal || typeDescription.type === typeNames.integer || typeDescription.type === typeNames.bool ? '?' : '';
  if (typeDescription.isArray) {
      nullablechar = '';
  }
  return `${typeDescription.customTypeName}${nullableChar}${(typeDescription.isArray ? '[]' : '')}`;
};

const serializeClassAsCS = ({ name, properties, isDictionaryExtension }) => {
  const output = [];
  output.push( `  public sealed class ${name} {` );
  
  if (isDictionaryExtension) {
      output.push( `    [JsonExtensionData]` );
      output.push( `    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();` );
  }

  for (let propDefinition of properties ) {
      output.push( `    [JsonProperty( "${propDefinition.propertyName}" )]` );
      output.push( `    public ${serializeTypeToCS( propDefinition.type )} ${propDefinition.name} { get; set; }` );
  }

  output.push( `  }` );
  return output;
}

const serializeTypeToTS = typeDescription => {
  const dict = {
      [typeNames.decimal]: 'float',
      [typeNames.object]: 'any',
      [typeNames.integer]: 'number',
      [typeNames.bool]: 'boolean'
  }
  return `${dict[typeDescription.type] || typeDescription.customTypeName}${(typeDescription.isArray ? '[]' : '')}`;
}

const serializeClassAsTS = ({ name, properties, isDictionaryExtension }) => {
  const output = [];
  output.push( `  export interface ${name} {` );
  
  if (isDictionaryExtension) {
      output.push( `    [x: string]: any;` );
  }

  for (let propDefinition of properties ) {
      output.push( `    ${propDefinition.propertyName}${(propDefinition.type.nullable ? '?' : '')}: ${serializeTypeToTS( propDefinition.type )};` );
  }

  output.push( `  }` );
  return output;
}

exports.serializeClassAsCS = serializeClassAsCS;
exports.serializeClassAsTS = serializeClassAsTS;