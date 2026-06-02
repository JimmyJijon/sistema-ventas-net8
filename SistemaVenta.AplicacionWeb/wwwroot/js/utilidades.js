function parseDecimal(valor) {
    // Reemplaza coma por punto
    let numero = parseFloat(valor.replace(',', '.'));

    // Valida que sea un número válido
    if (isNaN(numero)) {
        return 0;
    }

    return numero;
}