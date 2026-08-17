# Practica S02 - "Quita lo que sobra"

**Capacitacion OMNI TECH - Sabado 2 (DRY, KISS, YAGNI, ADRs)**
**Modalidad:** individual - **Tiempo estimado:** 120 a 150 minutos
**Entregable:** el proyecto simplificado + `RESPUESTAS.md` + 3 ADRs

---

## 1. De que se trata (lee esto primero)

En la S01 te dimos un `Program.cs` con TODO amontonado y tu trabajo fue
**separarlo** en capas.

Esta practica es el problema contrario.

Te entregamos un proyecto que **ya funciona** y que **ya esta separado en
capas**. El problema es que tiene demasiado de todo: fabricas para construir
objetos que se construyen con `new`, una interfaz generica para una sola
entidad, una clase base con hooks que nadie sobrescribe, un contrato de
transacciones sin base de datos y un notificador de correo que nadie llama.

Ademas, el calculo mas importante del sistema (el score de un post) esta
copiado en cuatro lugares distintos, y **las copias ya no coinciden**. Hay un
bug real, en produccion, causado exactamente por eso. Lo vas a ver con tus
propios ojos en el Paso 0.

**Tu trabajo NO es agregar capas ni patrones.**
Tu trabajo es:

```
QUITAR      lo que no resuelve un problema de hoy        (YAGNI)
UNIFICAR    el conocimiento que si esta duplicado        (DRY)
SIMPLIFICAR lo que es innecesariamente complicado        (KISS)
REGISTRAR   por que tomaste cada decision                (ADRs)
```

Si en la S01 aprendiste que separar es bueno, aqui vas a aprender que separar
de mas tambien es un defecto, y que cuesta igual de caro.

---

## 2. El contexto: la siguiente rebanada del Reddit-clone

En la S01 trabajaste con `Community`. Ahora trabajas con **`Post`**, sus votos
y sus comentarios.

**El score de un post** decide su lugar en el feed. Se calcula asi:

```
score base   = (upvotes - downvotes) * 10
              multiplicado por el tipo de post:
                  text  -> x1     (moderador: x1.2)
                  link  -> x1.15  (moderador: x1.35)
                  image -> x1.3   (moderador: x1.5)

mas          + comentarios * 3
mas          + premios:  1-2 premios -> +10
                         3-5 premios -> +25
                         6 o mas     -> +50
mas          + 1000 si el post esta fijado (pinned)
menos        - 30 si es NSFW y no esta fijado
menos        - 15 si esta bloqueado (locked)
menos        - 2 por cada hora transcurrida desde su creacion
tope         nunca baja de 0, salvo que este fijado
```

Esa formula completa es **una sola pieza de conocimiento**. Hoy vive
repartida en cuatro archivos y en tres versiones distintas.

---

## 3. Que hay en esta carpeta

```
practica-S02-2/
+-- todo.md                     <- este archivo (las instrucciones)
+-- RESPUESTAS.plantilla.md     <- copiala como RESPUESTAS.md y llenala
+-- preguntas-proceso.md        <- cuestionario de metodologia y deuda tecnica
+-- .gitignore
+-- codigo-inicial/             <- el codigo SOBRE-DISENADO que vas a podar
    +-- RedditClone.Posts.csproj
    +-- Program.cs
    +-- appsettings.json
    +-- Domain/          Post.cs  Comment.cs  Result.cs
    +-- Abstractions/    IRepository.cs  IPostRepository.cs  IPostFactory.cs
    |                    IUnitOfWork.cs  IEmailNotifier.cs
    |                    AbstractValidatorBase.cs
    +-- Infrastructure/  InMemoryPostRepository.cs  NoOpUnitOfWork.cs
    |                    ConsoleEmailNotifier.cs
    +-- Services/        PostService.cs  PostServiceFactory.cs  PostFactory.cs
    |                    PostRankingCalculator.cs
    |                    TitleValidator.cs  CommentBodyValidator.cs
    +-- docs/adr/        <- vacia: aqui vas a escribir tus 3 ADRs
```

Son **19 archivos `.cs`** para 6 endpoints. Cuentalos tu mismo y toma nota de
ese numero: al final de la practica lo vas a comparar.

```bash
# desde codigo-inicial/
find . -name "*.cs" -not -path "./obj/*" | wc -l
```

---

## 4. Paso 0 - Corre el codigo y encuentra el bug (15 min)

Antes de tocar nada, hazlo correr.

```bash
cd codigo-inicial
dotnet run
```

En la consola vas a ver el puerto (algo como `http://localhost:5000`).
Cambia el puerto por el tuyo en los comandos.

### 4.1 Caso normal (un post de texto)

```bash
# crear
curl -X POST http://localhost:5xxx/posts \
  -H "Content-Type: application/json" \
  -d '{ "title": "mi primer post", "body": "hola", "kind": "text" }'

# votar 3 veces
curl -X POST http://localhost:5000/posts/1/vote -H "Content-Type: application/json" -d '{ "value": 1 }'
curl -X POST http://localhost:5000/posts/1/vote -H "Content-Type: application/json" -d '{ "value": 1 }'
curl -X POST http://localhost:5000/posts/1/vote -H "Content-Type: application/json" -d '{ "value": 1 }'

# comentar
curl -X POST http://localhost:5000/posts/1/comments \
  -H "Content-Type: application/json" -d '{ "body": "buen post" }'

# consultar
curl http://localhost:5000/posts/1

# comparar el score guardado contra el calculador oficial
curl http://localhost:5000/posts/1/score-debug
```

Deberias ver `score: 33` por todos lados, y en `score-debug` los dos numeros
iguales. Hasta aqui todo bien.

### 4.2 El bug (un post de imagen con premios)

```bash
# crear un post de tipo imagen
curl -X POST http://localhost:5000/posts \
  -H "Content-Type: application/json" \
  -d '{ "title": "foto del gato", "body": "", "kind": "image" }'

# votarlo 4 veces
curl -X POST http://localhost:5000/posts/2/vote -H "Content-Type: application/json" -d '{ "value": 1 }'
curl -X POST http://localhost:5000/posts/2/vote -H "Content-Type: application/json" -d '{ "value": 1 }'
curl -X POST http://localhost:5000/posts/2/vote -H "Content-Type: application/json" -d '{ "value": 1 }'
curl -X POST http://localhost:5000/posts/2/vote -H "Content-Type: application/json" -d '{ "value": 1 }'

# darle 6 premios
curl -X POST http://localhost:5000/posts/2/awards
curl -X POST http://localhost:5000/posts/2/awards
curl -X POST http://localhost:5000/posts/2/awards
curl -X POST http://localhost:5000/posts/2/awards
curl -X POST http://localhost:5000/posts/2/awards
curl -X POST http://localhost:5000/posts/2/awards

# y ahora pregunta el score
curl http://localhost:5000/posts/2
curl http://localhost:5000/posts/2/score-debug
curl http://localhost:5000/posts
```

**El mismo post tiene dos scores distintos: 40 y 102.**

El post de la foto deberia estar arriba del post de texto en el feed, y esta
abajo. El autor reclama que su post con 6 premios "no sube". Nadie escribio un
`if` equivocado: el bug existe porque **la misma regla esta escrita en cuatro
lugares y solo uno de ellos esta completo**.

Anota en `RESPUESTAS.md` todos los lugares donde se calcula el score.
(Pista: busca el comentario `// Calculo del score`. Vas a encontrar cuatro,
repartidos en dos archivos. Y hay un quinto sitio que si lo calcula bien y no
lleva ese comentario: el calculador oficial.)

Eso es DRY explicado sin diapositivas.

---

## 5. Paso 1 - Inventario de abstracciones (15 min)

Antes de borrar nada, haz el inventario. Por **cada** interfaz y cada clase
base del proyecto, contesta una sola pregunta:

> Que problema concreto que existe HOY resuelve esto?

No vale responder "flexibilidad", "por si crecemos" ni "buenas practicas".
Vale responder cosas como "permite tener una implementacion en memoria para
las pruebas y otra real en produccion".

Llena la tabla del `RESPUESTAS.md`. Estas son las candidatas:

| Abstraccion                               | Donde vive                    |
| ----------------------------------------- | ----------------------------- |
| `IRepository<T>`                          | Abstractions                  |
| `IPostRepository`                         | Abstractions                  |
| `IPostFactory` + `PostFactory`            | Abstractions / Services       |
| `IUnitOfWork` + `NoOpUnitOfWork`          | Abstractions / Infrastructure |
| `IEmailNotifier` + `ConsoleEmailNotifier` | Abstractions / Infrastructure |
| `AbstractValidatorBase<T>`                | Abstractions                  |
| `PostServiceFactory`                      | Services                      |

**Aviso importante:** de esa lista, **una sola se queda**. Las demas se van.
Averigua cual antes de seguir, porque es la pregunta que vale mas puntos de
toda la practica.

---

## 6. Paso 2 - YAGNI: quita lo que sobra (30 min)

Copia `codigo-inicial/` a una carpeta `src/` y ahi trabaja. Ve borrando de una
en una y corre `dotnet build` despues de cada borrado.

### 6.1 La que se queda

`IPostRepository` **se queda**. No porque sea bonita, sino porque:

- sostiene el DIP: `PostService` no conoce `InMemoryPostRepository`,
- el dia que llegue PostgreSQL, cambias una linea en `Program.cs`,
- y retrofitearla despues significa tocar todos los servicios que ya la usan.

Esta es la excepcion de YAGNI que vimos en clase: **si el costo de agregarla
despues es mucho mayor que el de tenerla ahora, se agrega ahora.** No es que
YAGNI este mal; es que el calculo de costo da otro resultado.

### 6.2 Las que se van

| Que borrar                                                 | Por que                                                                                                                               |
| ---------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| `IRepository<T>` (y sus miembros en el repositorio)        | Hay una sola entidad. Y `Delete`, `Find` y `Count` no los llama nadie. Ademas viola ISP: obliga a implementar metodos que no se usan. |
| `IPostFactory` + `PostFactory`                             | Construye un objeto con `new`. Eso ya lo hace `new`.                                                                                  |
| `IUnitOfWork` + `NoOpUnitOfWork`                           | No hay base de datos ni transacciones. Hoy sus tres metodos estan vacios.                                                             |
| `IEmailNotifier` + `ConsoleEmailNotifier` + `NotifyAuthor` | Ningun flujo manda correos. Es codigo muerto con inyeccion incluida.                                                                  |
| `PostServiceFactory`                                       | Le pide al contenedor de DI lo que el contenedor ya inyecta solo.                                                                     |
| `AbstractValidatorBase<T>`                                 | Dos herederos y dos hooks (`OnBeforeValidate`, `OnAfterValidate`) que nadie sobrescribe. Herencia por herencia.                       |

Al quitar `AbstractValidatorBase<T>`, los dos validadores se vuelven clases
normales que devuelven `Result<string>`. Mas cortas y sin ceremonia.

### 6.3 Un smell extra que debes cazar

En `Program.cs` hay **dos registros** del repositorio en el contenedor de DI.
Encuentralos. Responde en `RESPUESTAS.md`:

> Cuantas instancias de `InMemoryPostRepository` viven en memoria al arrancar?
> Que pasaria si un servicio nuevo inyectara `IRepository<Post>` en vez de
> `IPostRepository`?

### 6.4 Y `appsettings.json`

Abre `appsettings.json`. Hay una seccion `Ranking` con `VoteWeight`,
`CommentWeight` y `HourlyDecay`. Compara esos valores contra los que usa el
codigo. Tienes dos salidas legitimas:

- **usarla de verdad** (leerla con `IConfiguration`), o
- **borrarla** porque nadie la lee.

Lo unico que no es aceptable es dejarla ahi mintiendo. Elige una y escribe por
que en `RESPUESTAS.md`.

**Meta del paso:** el proyecto debe pasar de 19 archivos `.cs` a **11 o
menos**, sin perder ni un endpoint. Reporta el numero final.

---

## 7. Paso 3 - DRY: unifica el calculo del score (30 min)

Ahora arregla el bug del Paso 0. La formula del score debe existir en **un
solo lugar**: `PostRankingCalculator`.

### 7.1 Quita las copias

Estos cinco lugares (en tres archivos) calculan el score hoy:

| Lugar                             | Que le falta a su copia                     |
| --------------------------------- | ------------------------------------------- |
| `PostRankingCalculator.Calculate` | nada, es la version completa                |
| `PostService.CreatePost`          | tipo de post, premios, banderas, antiguedad |
| `PostService.Vote`                | tipo de post, premios, banderas             |
| `PostService.AddComment`          | tipo de post, premios, banderas             |
| `Program.cs` en `GET /posts`      | todo lo anterior y ademas la antiguedad     |

Y hay un quinto lugar que **deberia** recalcular y no lo hace:
`PostService.GiveAward`. Por eso los premios no movian el score. Encuentralo
y arreglalo tambien.

Cuando termines, en todo el proyecto debe existir **una sola expresion**
que multiplique votos por su peso.

### 7.2 Los numeros magicos

Los valores 10, 3, 2, 1000, 50, 25, 10, 30, 15, 1.2, 1.15, 1.3, 1.35, 1.5
estan escritos a mano. Dales nombre y ponlos en un solo lugar:

```csharp
// Domain/RankingRules.cs  (o dentro del calculador, tu decides y lo justificas)
public static class RankingRules
{
    public const double VoteWeight = 10;
    public const double CommentWeight = 3;
    public const double HourlyDecay = 2;
    public const double PinnedBonus = 1000;
    public const double NsfwPenalty = 30;
    public const double LockedPenalty = 15;
    // TODO: los multiplicadores por tipo y los escalones de premios
}
```

### 7.3 La prueba de que quedo bien

Repite el Paso 0 completo. Ahora `GET /posts/2`, `GET /posts/2/score-debug` y
`GET /posts` deben devolver **el mismo numero** para el post 2. El post de la
foto debe aparecer arriba en el feed.

---

## 8. Paso 4 - La trampa: lo que NO debes unificar (10 min)

Abre `Services/TitleValidator.cs` y `Services/CommentBodyValidator.cs`.

Las dos clases se ven casi identicas: requerido, minimo 3, maximo 300. La
tentacion es obvia: hacer un `TextLengthValidator(min, max)` y usarlo en los
dos lados.

**No lo hagas.** Y esta es la parte de la practica que mas importa.

DRY habla de **conocimiento** duplicado, no de **codigo** parecido. Aqui hay
dos reglas de negocio distintas que hoy coinciden por casualidad:

- el titulo esta acotado a 300 **porque se muestra completo en el feed**,
- el comentario esta acotado a 300 **porque los hilos largos se vuelven
  ilegibles**.

Son dos razones diferentes. El dia que producto decida que los comentarios
pueden tener 10 000 caracteres, si las unificaste tienes que desenredarlas, y
mientras tanto alguien va a "arreglar" el titulo sin darse cuenta.

**Regla practica:** dos codigos son duplicacion real solo si **cambian juntos,
por la misma razon**. Si pueden cambiar por separado, dejalos separados.

Tarea: en `RESPUESTAS.md` explica con tus palabras por que el score SI se
unifica y estos dos validadores NO. Esa diferencia es todo el tema del dia.

> Compara: la formula del score cambia en un solo lugar porque **es la misma
> decision de negocio** aplicada en cuatro pantallas. Los validadores cambian
> en lugares distintos porque **son dos decisiones de negocio distintas** que
> hoy dan el mismo numero.

---

## 9. Paso 5 - KISS: baja la complejidad ciclomatica (30 min)

Abre `Services/PostRankingCalculator.cs`. Cuenta los `if` (cada `if` es un
punto de decision; la complejidad es puntos de decision + 1).

Vas a contar **18**. Segun los umbrales que vimos en clase:

| Complejidad | Veredicto            |
| ----------- | -------------------- |
| 10 o menos  | aceptable            |
| 11 a 15     | revisar              |
| mas de 15   | refactor obligatorio |

Este metodo esta en la tercera fila. **Tu meta: dejarlo en 10 o menos** sin
cambiar ni un solo resultado.

Tres tecnicas, en este orden:

**1. Guard clause.** Todo el cuerpo esta dentro de un `if (post is not null)`.
Invierte la condicion y sal temprano. Se va un nivel de anidacion completo.

```csharp
if (post is null)
{
    return 0;
}
```

**2. Tabla en vez de arbol.** Los tres `if` anidados de `Kind` mas los tres de
`AuthorIsModerator` son seis puntos de decision para elegir un numero. Eso es
un diccionario:

```csharp
private static readonly Dictionary<string, double> KindMultiplier = new()
{
    ["text"] = 1.0,
    ["link"] = 1.15,
    ["image"] = 1.3
};

// TODO: el bono de moderador tambien es una tabla (o un factor aparte).
//       Cuidado: 1.2 / 1.35 / 1.5 no son "el mismo bono" para los tres tipos.
//       Revisa la aritmetica antes de asumir que puedes multiplicar por 1.2.
```

**3. Escalones en vez de ifs anidados.** Los premios son tres rangos. Una
tabla ordenada recorrida de mayor a menor resuelve los tres `if` de golpe.

**Cuidado con pasarte de listo:** si para bajar la complejidad terminas
creando `IScoreRuleStrategy`, `ScoreRuleChain` y cinco clases, acabas de
cambiar un metodo largo por una arquitectura larga. El objetivo es que
**se lea mas facil**, no que tenga mas piezas. Simple no es lo mismo que
disperso.

Reporta en `RESPUESTAS.md` la complejidad antes y despues, y como la contaste.

---

## 10. Paso 6 - Los ADRs (20 min)

Acabas de tomar tres decisiones de diseno que el proximo que llegue al
proyecto no va a entender. Registralas. Formato MADR, un archivo por ADR, en
`docs/adr/`.

```
docs/adr/0001-no-unificar-validadores-de-texto.md
docs/adr/0002-eliminar-repositorio-generico.md
docs/adr/0003-un-solo-calculo-de-score.md
```

Plantilla (la misma del hands-on):

```markdown
# ADR-000X - [Titulo de la decision]

- Estado: aceptado
- Fecha: AAAA-MM-DD

## Contexto

Que situacion nos obligo a decidir. Que restricciones habia.

## Decision

Que decidimos. En presente y en voz activa: "Usamos X", "Eliminamos Y".

## Consecuencias

Lo bueno Y lo malo. Un ADR sin consecuencias negativas es publicidad,
no un ADR.
```

Que debe contener cada uno:

| ADR  | Debe responder                                                                                                                                         |
| ---- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 0001 | Por que `TitleValidator` y `CommentBodyValidator` siguen separados si su codigo es casi identico. Que pasaria si los unifieramos y despues cambia uno. |
| 0002 | Por que se elimino `IRepository<T>` y por que `IPostRepository` sobrevivio. Cual es el criterio que separa a una de la otra.                           |
| 0003 | Donde vive ahora la formula del score y por que ahi. Que se sacrifico (por ejemplo: el listado ya no puede ordenar con una consulta simple).           |

**La parte de Consecuencias es obligatoria y tiene que doler.** Si las tres
tuyas dicen "el codigo queda mas limpio", no son consecuencias, son deseos.

---

## 11. Que debe seguir funcionando (y que SI debe cambiar)

**Igual que antes:**

- `POST /posts` con titulo valido -> 200 con `{ id, title, kind, score }`
- `POST /posts` con titulo de menos de 3 caracteres -> 400
- `GET /posts/{id}` existente -> 200 ; inexistente -> 404
- `POST /posts/{id}/vote` con 1 o -1 -> 200 ; con otro valor -> 400 ; post inexistente -> 404
- `POST /posts/{id}/comments` valido -> 200 ; cuerpo corto -> 400 ; post inexistente -> 404
- El post 1 del Paso 0 (texto, 3 votos, 1 comentario) sigue dando **33**

**Diferente a proposito (es el bug que arreglaste):**

- El post 2 (imagen, 4 votos, 6 premios) ahora da **102** en los tres
  endpoints, no 40 en unos y 102 en otro.
- El listado `GET /posts` ahora respeta el tipo de post, los premios y la
  antiguedad.

Si algo mas cambio de comportamiento, no era parte del ejercicio: revisalo.

---

## 12. Entregable

1. La carpeta `src/` con el proyecto **simplificado y compilando**
   (`dotnet build` sin errores ni warnings).
2. `docs/adr/` con los 3 ADRs.
3. `RESPUESTAS.md` (copia de la plantilla) completo.
4. `preguntas-proceso.md` contestado.
5. Un commit con mensaje en formato conventional commits, por ejemplo:
   `refactor(posts): unificar calculo de score y eliminar abstracciones especulativas`

---

## 13. Rubrica (10 puntos)

| Criterio                                                                                                                           | Pts |
| ---------------------------------------------------------------------------------------------------------------------------------- | --- |
| Compila sin warnings y los endpoints del punto 11 responden como se indica                                                         | 2   |
| El calculo del score existe en UN solo lugar; los 5 puntos que lo tocaban quedaron unificados y `GiveAward` recalcula              | 2   |
| NO unifico `TitleValidator` con `CommentBodyValidator` y lo justifico por escrito con el criterio correcto (misma razon de cambio) | 2   |
| Complejidad del calculador reducida a 10 o menos, sin cambiar resultados y sin inventar una jerarquia de clases                    | 1   |
| Abstracciones especulativas eliminadas (11 archivos `.cs` o menos) conservando `IPostRepository` con argumento de costo            | 1   |
| Los 3 ADRs en formato MADR, con consecuencias negativas reales                                                                     | 1   |
| `RESPUESTAS.md` completo (inventario, numeros magicos, doble registro de DI, complejidad antes/despues)                            | 1   |

**Descuento:** -1 punto por cada abstraccion nueva que agregues y no puedas
justificar con un problema de hoy. Aqui se puede sacar menos de lo que se
empieza.

---

## 14. Reto extra (opcional, +1)

Producto cambia de opinion: los posts de tipo `video` deben valer x1.4, y los
comentarios ahora pesan 5 en vez de 3.

Implementa los dos cambios y cuenta **cuantos archivos tuviste que tocar**.

- 1 archivo: tu DRY quedo bien.
- 2 archivos: aceptable, explica por que.
- 3 o mas: todavia tienes conocimiento duplicado. Encuentralo.

Anota la cifra en `RESPUESTAS.md`.

---

## 15. Reglas que NUNCA violar (del AGENTS.md del proyecto)

- NO string interpolation en Serilog/ILogger. Correcto:
  `_logger.LogInformation("Post {PostId} creado", id)`
- NO `.Result` ni `async void`.
- NO `IRepository<T>` generico. Una interfaz por entidad.
- NO hardcodear connection strings ni secretos.
- NO registrar un servicio Scoped dentro de un Singleton.
- NO agregar un patron de diseno "para que se vea profesional". Si no
  resuelve un problema de hoy, es deuda tecnica con mejor vocabulario.

---

## 16. Como saber que terminaste

- [ ] `dotnet build` sin errores ni warnings.
- [ ] El proyecto tiene 11 archivos `.cs` o menos.
- [ ] Busco `Upvotes - ` en todo el proyecto y la resta aparece **una sola
      vez**: en el calculador.
- [ ] Los tres endpoints del post 2 devuelven el mismo score.
- [ ] `TitleValidator` y `CommentBodyValidator` siguen siendo dos clases.
- [ ] El calculador tiene complejidad 10 o menos y no cree ni una clase nueva
      para lograrlo.
- [ ] Los 3 ADRs estan en `docs/adr/` y cada uno tiene una consecuencia que
      duele.
- [ ] `RESPUESTAS.md` y `preguntas-proceso.md` entregados.
