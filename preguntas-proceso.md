# Cuestionario de proceso - S02

**Metodologia, ciclo de vida y deuda tecnica**
**Complemento de la practica "Quita lo que sobra"**

Estas preguntas no se contestan escribiendo codigo, pero salen del mismo
trabajo que acabas de hacer. Contesta despues de terminar la practica: varias
te piden mirar tus propias decisiones.

**Nombre:** Junior Andres Hernandez Villanueva
**Fecha:** 17/08/26

---

## Bloque 1 - Agile y Scrum (opcion multiple)

**1.** Un sprint de 2 semanas termina el viernes. La historia esta
programada, compila, pero el PR sigue abierto sin revisar. Que es?

- a) Done, el codigo ya esta escrito
- b) Done parcial, cuenta al 80% en el burndown
- c) No Done, no cumple la Definicion de Done del equipo
- d) Done si el desarrollador lo demuestra en la review

**2.** Quien decide el orden de los items del Product Backlog?

- a) El Scrum Master
- b) El Product Owner
- c) El Tech Lead
- d) El Dev Team por votacion

**3.** Cual es el resultado esperado de un sprint?

- a) Todas las historias comprometidas cerradas
- b) Un incremento potencialmente liberable
- c) Un reporte de avance para el cliente
- d) La documentacion tecnica actualizada

**4.** El Daily Standup de tu equipo dura 45 minutos y se convierte en
sesion de debugging grupal. Que esta fallando y quien deberia corregirlo?

Tu respuesta:

---

## Bloque 2 - Definicion de Done

**5.** De la lista siguiente, marca lo que SI forma parte de una DoD
razonable para el CRM Messenger. Puede haber varias.

- [ ] Codigo en rama de feature, compilando
- [ ] Tests unitarios pasando
- [-] Code review aprobado por al menos un companero
- [-] Build de CI en verde
- [-] Cobertura de tests del 100%
- [ ] Merged a `develop`
- [ ] El desarrollador dice que ya quedo
- [-] Criterios de aceptacion validados en el PR

**6.** Por que "funciona en mi maquina" no puede ser parte de la DoD?
Contesta en dos renglones.

Tu respuesta: Ya que ante configuraciones especificas del proyecto de manera local
Puede compilar en "MI EQUIPO", mas no quiere decir que sea codigo aun apto para hacer un PR o merge final

**7.** Aplicado a esta practica: si el refactor que hiciste fuera una
historia del sprint, escribe la DoD especifica que le pondrias. Minimo 4
puntos verificables (algo que otra persona pueda comprobar sin preguntarte).

Tu respuesta:

1. compila en su maquina
2. CI en verde
3. da los resultados esperados de acuerdo a los requisitos del proyecto en OTRO EQUIPO
4. cumple con todos los test y pruebas

---

## Bloque 3 - Ciclo de vida del software

**8.** En un equipo que trabaja con Scrum, donde ocurre la fase de testing?

- a) En un sprint dedicado al final del release
- b) En una fase posterior al desarrollo, a cargo de QA
- c) **Dentro de cada sprint, como parte de la Definicion de Done**
- d) Solo antes de cada despliegue a produccion

**9.** El bug del score que encontraste en el Paso 0 llego a produccion.
Ubica en que fase del ciclo de vida se pudo haber detectado mas barato, y
que practica concreta lo habria atrapado ahi.

| Fase           | Se pudo detectar ahi? | Con que practica |
| -------------- | --------------------- | ---------------- |
| Requerimientos |                       |                  |
| Diseno         |                       |                  |
| Desarrollo     |                       |                  |
| Testing        |                       |                  |
| Despliegue     |                       |                  |

**10.** Scrum no elimina las fases del ciclo de vida. Que hace con ellas?

Tu respuesta: Da dirección, y un fudamento valido, comprobable y sobre todo que garantiza que se cumpla, las incluye e indica en que momento del desarrollo es necesario llevar a cabo cada paso del clo de vida

---

## Bloque 4 - Deuda tecnica

**11.** Clasifica cada defecto que encontraste en la practica usando el
cuadrante de Fowler. Marca una casilla por fila y justifica en una linea.

| Defecto                                                  | Deliberada / Accidental | Prudente / Imprudente | Por que |
| -------------------------------------------------------- | ----------------------- | --------------------- | ------- |
| La formula del score copiada en 5 lugares                |                         |                       |         |
| `IRepository<T>` generico para una sola entidad          |                         |                       |         |
| `PostRankingCalculator` con complejidad 18               |                         |                       |         |
| `IUnitOfWork` sin base de datos                          |                         |                       |         |
| La seccion `Ranking` de `appsettings.json` que nadie lee |                         |                       |         |
| `GiveAward` que no recalcula el score                    |                         |                       |         |

**12.** Cual de los seis anteriores cobro intereses mas caros, y como los
cobro? (Pista: uno de ellos produjo un bug visible para el usuario.)

Tu respuesta:

**13.** La deuda deliberada y prudente es una herramienta de gestion
legitima. Escribe un `// TODO` bien formado para una deuda que SI dejarias
en este proyecto a proposito, con razon y con plan de pago.

```csharp
// TODO:
```

**14.** Verdadero o falso, y explica:

> "Agregar `IRepository<T>`, `IUnitOfWork` y `IPostFactory` desde el dia uno
> es invertir en el futuro, no contraer deuda."

Tu respuesta:

**15.** Boy Scout Rule. Durante la practica tocaste archivos por una razon y
te encontraste con problemas de otra. Nombra una mejora pequena que hiciste
de paso, y una que decidiste NO hacer para no salirte del alcance. Como
decidiste donde estaba la linea?

Tu respuesta:

---

## Bloque 5 - Cierre

**16.** El proximo sprint entra una funcionalidad nueva: posts de tipo
`video`. Escribe como lo llevarias al backlog: titulo de la historia,
criterios de aceptacion y si generas o no un ADR. Maximo 10 renglones.

Tu respuesta:
