# Initial Assessment Sequence

**DEPRECATED**: This standalone script has been superseded by the modular system.

## New Usage

Use the composer from the parent directory:

```bash
cd ..
./render.sh -s initial_assessment
```

Or:

```bash
blender --background --python ../compose.py -- --sequence initial_assessment
```

## Why Modular?

The modular system in `../procedures/` allows:
- Reusing individual procedures in different sequences
- Composing procedures in any order
- Sharing models and materials across procedures
- Easy addition of new procedures

See `../README.md` for full documentation.
