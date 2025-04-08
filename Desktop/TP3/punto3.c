#include <stdio.h>
#include <stdlib.h>
#include <time.h>

char *TiposProductos[]={"Galletas","Snack", "Cigarrillos", "Caramelos", "Bebidas"};

struct Producto {
    int ProductoID; //Numerado en ciclo iterativo
    int Cantidad; // entre 1 y 10
    char *TipoProducto; // Algún valor del arreglo TiposProductos
    float PrecioUnitario; // entre 10 - 100
} typedef Producto;

struct Cliente {
    int ClienteID; // Numerado en el ciclo iterativo
    char *NombreCliente; // Ingresado por usuario
    int CantidadProductosAPedir; // (aleatorio entre 1 y 5)
    Producto *Productos //El tamaño de este arreglo depende de la variable cantidad de productos a pedir
} typedef Cliente;


int main (){
    
    Cliente *clientes;
    int cantClientes;

    printf ("Ingrese la cantidad de clientes: ");
    scanf ("%i", &cantClientes);

    clientes = (Cliente *) malloc (cantClientes * sizeof(Cliente));

    printf ("Cargado de datos,\n");
    int i;

    for (i = 1; i <= cantClientes; i++){
        clientes[i].ClienteID = i;
        clientes[i].CantidadProductosAPedir = 1 + rand () % 5;
        printf ("Ingresa el nombre del cliente %i");
        gets(clientes[i].NombreCliente);
        
    }


    return 0;
}