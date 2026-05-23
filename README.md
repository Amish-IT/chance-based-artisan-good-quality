# Chance-Based Artisan Good Quality
*Chance-Based Artisan Good Quality* allows artisan goods to sometimes inherit the quality of their inputs.

Requires *SMAPI*.

By default, Silver inputs produce Silver outputs 50% of the time, Gold inputs produce Gold outputs 25% of the time, and Iridium inputs produce Iridium outputs 12% of the time.  If an item does not retain its input quality, it instead becomes one level lower: Silver inputs produce 0-quality outputs 50% of the time, Gold inputs produce Silver outputs 75% of the time, and Iridium inputs produce Gold outputs 88% of the time.  Inputs that would produce an item with quality in vanilla (e.g.: large eggs in a mayonnaise machine) instead follow the chances above and produce an additional output item.

Several config options are available, as well as support for Generic Mod Config Menu.  These allow you to set your own chances for inheriting quality, enable cascading downgrades, and change what happens when producing an item that would have quality in vanilla.
