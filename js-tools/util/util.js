function toTypeName( name ) {
    if ( name.endsWith('es') ) {
        return toTypeName( name.substr(0, name.length - 1) );
    }
    if ( name.endsWith('ie') ) {
        return toTypeName( name.substr(0, name.length - 2 ) + 'y' );
    }
    const invalidFirstChars = '-/+%?.=()\\|&{}°~@!^`$ ';
    if ( invalidFirstChars.includes( name.charAt(0) ) ) {
        return toTypeName( name.substr( 1 ) );
    }
    return sanitizePropertyName( name );
}

function sanitizePropertyName( name ) {
    const validPartOfName = /[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*/ig;
    let result = '';
    let matches = validPartOfName.exec( name );
    while (matches) {
        result += matches.reduce( (concat, match, i, arr) => {
            if ( !match ) {
                return concat;
            }
            if (i > 0 && arr[i] === arr[i-1]) {
                return concat;
            }
            return concat + toCamlCase( match );
        }, '');
        matches = validPartOfName.exec( name );
    }
    return result;
}

function toCamlCase( name ) {
    return name.charAt( 0 ).toUpperCase() + name.substr( 1 );
}

function isString( value ) {
    return Object.prototype.toString.call( value ) === '[object String]';
}

function parseOptions( args, options ) {
    const realArguments = args.splice( 2 );
    const required = options.filter( o => o.required );
    const switches = Object.assign( {}, ...options.map( o => ({ [o.shortcut]: o, [o.arg]: o }) ) );
    const defaultProperty = options.find( o => o.isDefault );
    let i = 0;
    let result = Object.assign( {}, ...options.filter( o => !!o.defaultValue ).map( v => ({ [v.property]: v.defaultValue }) ) );
    while (i < realArguments.length) {
        let arg = realArguments[i];
        i++;
        let argKey = arg.split(':')[0];
        let argValue = arg.split(':')[1];
        if (argKey.charAt(0) === '-') {
            let match = switches[argKey.substr(1)];
            if (!match) {
                console.warn(`${argKey} is an invalid switch`);
                continue;
            }
            if (argValue) {
                result[match.property] = argValue;
                continue;
            } else {
                if (match.value !== undefined) {
                    result[match.property] = match.value;
                    continue;
                }
                if (realArguments[i] && realArguments[i].charAt(0) !== '-') {
                    result[match.property] = realArguments[i];
                    i++;
                    continue;
                }
            }
            console.warn(`${argKey} doesn't seem to have a value set`);
            continue;
        }
        if ( defaultProperty ) {
            if (result[defaultProperty.property]) {
                console.warn(`${defaultProperty.property} is already set on ${result[defaultProperty.property]}`);
                continue;
            }
            result[defaultProperty.property] = arg;
        }
    }
    if (required.some( p => result[p.property] === undefined ) ) {
        throw 'ParseException: not all required options are set';
    }
    return result;
}

exports.toCamlCase = toCamlCase;
exports.toTypeName = toTypeName;
exports.isString = isString;
exports.sanitizePropertyName = sanitizePropertyName;
exports.parseOptions = parseOptions;