Levels in The Wrath of Cortex are made up of either of the following file formats:

- NUS - Level model, textures, splines and other
- TER - Terrain, i'm assuming it's the collision mesh
- CRT - Crates
- WMP - Wumpa fruit
- AI  - entities and generally things with AI
- LGT - lights
- VIS - no clue, levels can work without these (apparently called visi-tables. culling data maybe?)
- PAD - NTSC demo file (?)
- PDD - PAL demo file (?)
- Particle system:
  - PTL - Particles
  - CPT
  - GRA - Grass
  - OBJ
  - ANM

Other file formats:
- HGO - object models, characters, etc
- ANI - object animations
- BSA - something to do with object animations
- SHD - Shader data
- ATS - Animated Texture Script (?)
- FNT - possibly unused, paired with BMP
- BMP - standard windows bitmap (big endian on gamecube)

WMP - Wumpa Fruit

```
wumpaCount: u32;
wumpaFruit: {
	X: f32;
	Y: f32;
	Z: f32;
} * wumpaCount;
```

AI - Entities 

```
groupCount: u32;
groups: {
	type: strz(16);
	aiCount: u32;
	ais: {
		X: f32;
		Y: f32;
		Z: f32;
	} * aiCount;
} * groupCount;
```

NUS - Graphics

```
blocks: {
	type: u32;
	size: u32;
	data: BlockData<type>;
} * EOF;

BlockData<GSC0>: {} // Empty block, 'size' field holds the size of the entire file
BlockData<NTBL>: {
	size: u32;
	data: strz ** size; // a block of data with a size of -size- bytes holding zero terminated strings
	padding: 0 * EOB; // end of block; until block.size is filled
}

BlockData<TST0>: {} // TODO
BlockData<MS00>: {
	count: u32;
	materials: {
		unknown1bd: u8 * 20;
		// thanks Ilupuusikuniluusi
		diffuseR: f32;
		diffuseG: f32;
		diffuseB: f32;

		unknown4bd: u8 * 16;
		unknown5f: f32;
		unknown6f: f32;
		textureIndex: u32;
		unknown7bd: u8 * 24;
	} * count;
}
Vertex<0x59>: {
	x: f32;
	y: f32;
	z: f32;
	
	nx: f32;
	ny: f32;
	nz: f32;
	
	color: u32;
	
	u: f32;
	v: f32;
}
Primitive: {
	type: u32;
	length: u32;
	data: u16 * length;

	// type == 5: TriangleList;
	// type == 6: TriangleStrip;
}
BlockData<GST0>: {
	count: u32;
	geometryObjects: {
		unkCount: u32;
		unkData: {
			type: u32;
			data: switch(type)
			{
                0: { // Geometry definition
					vertexCount: u32;
					vertices: Vertex<0x59> * vertexCount;
					cntrl: u32; // ???
					
					primitiveCount: u32;
					primitives: Primitive * primitiveCount;
					
					skinCount: u32;
					skins: {} * skinCount; // TODO
					
					blendShapeCount: u32;
					blendShapes: {} * blendShapeCount; // TODO
                }
                1: { // Face definition
					unknown1u: u32;
					count: u32;
					unknown2u: u32;
					unknown3f: f32;
					data: {u8 * 0x18} * count;
                }
			}
		} * unkCount;
	} * count;
	padding: 0 * EOB;
}
BlockData<INST>: {
	count1: u32;
	instances: {
		matrix: f32 * 16;
		modelIndex: u32;
		unknown: u8 * 12;
	} * count1;
	
	count2: u32;
	unknown: {u8 * 0x60} * count2;
}
BlockData<SPEC>: {
	count: u32;
	data: {u8 * 0x50} * count;
}
BlockData<SST0>: { // TODO
	count: u32;
	dataLength: u32;
	data: u8 * dataLength;
}
BlockData<TAS0>: {} // TODO
BlockData<ALIB>: {} // TODO
BlockData<LDIR>: {} // TODO
BlockData<SPHE>: {} // TODO

```