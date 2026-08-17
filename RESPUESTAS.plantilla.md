# RESPUESTAS - Practica S02 "Quita lo que sobra"

**Nombre:**
**Fecha:**

> Copia este archivo como `RESPUESTAS.md` y llena cada seccion.

---

## 1. El bug del Paso 0

Que score devolvio cada endpoint para el post 2 (imagen, 4 votos, 6 premios)?

| Endpoint | Score que devolvio |
|----------|--------------------|
| `GET /posts/2` | |
| `GET /posts/2/score-debug` (guardado / oficial) | |
| `GET /posts` (listado) | |

En que lugares del codigo se calcula el score? Anota archivo y metodo, y que
le falta a cada copia.

| # | Archivo y metodo | Que le falta a su version de la formula |
|---|------------------|------------------------------------------|
| 1 | | |
| 2 | | |
| 3 | | |
| 4 | | |
| 5 | | |

Cual es el lugar que **deberia** recalcular el score y no lo hace?

Tu respuesta:

---

## 2. Inventario de abstracciones (Paso 1)

Por cada una: que problema **de hoy** resuelve? Se queda o se va?

| Abstraccion | Que problema real de hoy resuelve | Se queda / Se va | Por que |
|-------------|-----------------------------------|------------------|---------|
| `IRepository<T>` | | | |
| `IPostRepository` | | | |
| `IPostFactory` + `PostFactory` | | | |
| `IUnitOfWork` + `NoOpUnitOfWork` | | | |
| `IEmailNotifier` + `ConsoleEmailNotifier` | | | |
| `AbstractValidatorBase<T>` | | | |
| `PostServiceFactory` | | | |

**La excepcion de YAGNI.** Solo una se queda. Explica el criterio de costo:
por que agregarla despues saldria mas caro que mantenerla hoy, y por que ese
mismo argumento NO aplica a las otras seis.

Tu respuesta:

---

## 3. Conteo de archivos

| Momento | Archivos `.cs` |
|---------|----------------|
| Antes (codigo-inicial) | |
| Despues (src) | |

---

## 4. El doble registro en el contenedor de DI (Paso 2.3)

Cuantas instancias de `InMemoryPostRepository` viven en memoria al arrancar la
app original? Por que?

Tu respuesta:

Que pasaria si un servicio nuevo inyectara `IRepository<Post>` en vez de
`IPostRepository` y guardara un post?

Tu respuesta:

---

## 5. La seccion `Ranking` de `appsettings.json` (Paso 2.4)

Que decidiste: usarla de verdad o borrarla? Por que?

Tu respuesta:

Que valor de `appsettings.json` NO coincide con el que usa el codigo? Que te
dice eso sobre configuracion que nadie lee?

Tu respuesta:

---

## 6. Numeros magicos (Paso 3.2)

Donde quedaron los pesos y penalizaciones? Nombra el archivo y explica por que
ahi y no en otro lado.

Tu respuesta:

---

## 7. La trampa de DRY (Paso 4) - LA PREGUNTA CLAVE

`TitleValidator` y `CommentBodyValidator` tienen practicamente el mismo
codigo. La formula del score tambien estaba repetida.

Uno se unifica y el otro no. Explica la diferencia con tus palabras:

Tu respuesta:

Completa la regla:

> Dos codigos son duplicacion real solo si ______________________________
> ______________________________________________________________________

Da un ejemplo **de tu propio trabajo** (cualquier proyecto) donde unificaste
dos cosas que solo se parecian, y que te costo despues:

Tu respuesta:

---

## 8. KISS - complejidad ciclomatica (Paso 5)

| | Complejidad |
|---|---|
| `Calculate` antes | |
| `Calculate` despues | |

Como la contaste?

Tu respuesta:

Que tecnica te quito mas complejidad: la guard clause, la tabla de
multiplicadores o los escalones de premios?

Tu respuesta:

Consideraste crear clases nuevas (una estrategia, una cadena de reglas) para
bajar la complejidad? Por que decidiste hacerlo o no hacerlo?

Tu respuesta:

---

## 9. Verificacion final

- [ ] `dotnet build` sin errores ni warnings
- [ ] Post 1 (texto, 3 votos, 1 comentario) sigue dando 33
- [ ] Post 2 (imagen, 4 votos, 6 premios) da 102 en los tres endpoints
- [ ] `GET /posts` ordena la foto arriba del post de texto
- [ ] Los 3 ADRs estan en `docs/adr/`

Si algo no cumple, anota que falto y por que:

Tu respuesta:

---

## 10. (Opcional) Reto extra

Agregaste el tipo `video` (x1.4) y cambiaste el peso de comentarios de 3 a 5.

**Archivos que tuviste que tocar:**

Si fueron mas de dos, donde estaba el conocimiento duplicado que se te paso?

Tu respuesta:
