function camelize(str) {
    return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
        return index === 0 ? match.toLowerCase() : match.toUpperCase();
    });
}