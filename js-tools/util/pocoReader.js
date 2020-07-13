const { toTypeName, sanitizePropertyName, isString } = require('./util');

function createProperty( name, propertyName, type = 'string') {
    return { name: sanitizePropertyName( name ), type, propertyName: propertyName || name };
}

const readFromJson = ( jsonData, name ) => {
    if (!jsonData) {
        throw `ArgumentNullException: can't create a structure for a null object for ${name}`;
    }
    if (typeof jsonData !== 'object') {
        throw `ArgumentException: can't read the properties of simple object for ${name}`;
    }
    const keys = Object.keys( jsonData );
    const isDictionaryExtension = keys.some( key => !toTypeName( key ) );
    if ( isDictionaryExtension ) {
        console.warn( `Object contains at least 1 invalid key` );
    }

    return keys.filter( key => !!toTypeName( key ) ).reduce( ( classDefinition, property ) => {
        const propertyValue = jsonData[property];
        if ( propertyValue === null ) {
            console.info( `${property} on ${name} is defined as null` );
            classDefinition.properties.push( createProperty( property ) );
            return classDefinition;
        }
        if ( Array.isArray( propertyValue ) ) {
            if ( propertyValue.length === 0 ) {
                console.info( `${property} on ${name} is defined as an empty array` );
                classDefinition.properties.push( createProperty( property, property, 'JArray' ) );
                return classDefinition;
            }
            const mergedDefinition = { classes: [], properties: [], name: toTypeName( property ) };
            for (let item of propertyValue ) {
                const itemDefinition = readFromJson( item, property );
                const classDict = itemDefinition.classes.reduce( (agg, def) => (agg[def.name] = def, agg ), {} );
                const propDict = itemDefinition.properties.reduce( (agg, def) => (agg[def.name] = def, agg ), {} );

                for ( let existingClass of mergedDefinition.classes ) {
                    if (!classDict[existingClass.name]) {
                        continue;
                    }
                    Object.assign( existingClass, classDict[existingClass.name] );
                    delete classDict[existingClass.name];
                }
                mergedDefinition.classes.push( ...Object.values( classDict ) );

                for ( let existingProp of mergedDefinition.properties ) {
                    if (!propDict[existingProp.name]) {
                        continue;
                    }
                    if ( existingProp.type === 'decimal' && propDict[existingProp.name].type === 'int' ) {
                        // decimal wins
                        propDict[existingProp.name].type = 'decimal';
                    }
                    if ( existingProp.type === 'decimal' && propDict[existingProp.name].type === 'string' ) {
                        // becomes nullable decimal
                        propDict[existingProp.name].type = 'decimal?';
                    }
                    if (existingProp.type !== 'string' && propDict[existingProp.name].type === 'string' ) {
                        propDict[existingProp.name].type = existingProp.type;
                    }
                    Object.assign( existingProp, propDict[existingProp.name] );
                    delete propDict[existingProp.name];
                }
                mergedDefinition.properties.push( ...Object.values( propDict ) );
            }
            classDefinition.classes.push( mergedDefinition );
            console.info(`adding ${property} from ${name}` );
            classDefinition.properties.push( createProperty( property, property, toTypeName( property ) + '[]' ) );
            return classDefinition;
        }
        if ( propertyValue === true || propertyValue === false ) {
            classDefinition.properties.push( createProperty( property, property, 'bool' ) );
            return classDefinition;
        }
        if ( isString( propertyValue ) ) {
            classDefinition.properties.push( createProperty( property, property, 'string' ) );
            return classDefinition;
        }
        if ( Number.isInteger( propertyValue ) ) {
            classDefinition.properties.push( createProperty( property, property, 'int' ) );
            return classDefinition;
        }
        if ( !Number.isNaN( parseFloat( propertyValue ) ) ) {
            classDefinition.properties.push( createProperty( property, property, 'decimal' ) );
            return classDefinition;
        }
        // it should be an object
        classDefinition.classes.push( readFromJson( propertyValue, property ) );
        console.info(`adding ${property} from ${name}` );
        classDefinition.properties.push( createProperty( property, property, toTypeName( property ) ) );
        return classDefinition;
    }, { name: toTypeName( name ), properties: [], classes: [], isDictionaryExtension } );
};

exports.readFromJson = readFromJson;