namespace SistemaVenta.AplicacionWeb.Utilidades.Response 
{
    public class GenericResponse<TObject> //clase cuyo objetivo sera permitir estandarizar las respuestas de la aplicacion
    {
        public bool Estado { get; set; } //indica si la operacion fue exitosa o no
        public string? Mensaje { get; set; } //mensaje informativo sobre la operacion realizada

        public TObject? Objeto { get; set; } //objeto generico que puede contener cualquier tipo de dato

        public List<TObject>? ListaObjeto { get; set; } //lista generica que puede contener multiples objetos del mismo tipo
    }
}
