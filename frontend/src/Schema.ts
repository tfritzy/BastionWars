export const enum PacketType {
  UPDATE = "UPDATE",
  HEARTBEAT = "HEARTBEAT",
}

export const encodePacketType: { [key: string]: number } = {
  UPDATE: 0,
  HEARTBEAT: 1,
};

export const decodePacketType: { [key: number]: PacketType } = {
  0: PacketType.UPDATE,
  1: PacketType.HEARTBEAT,
};

export const enum TileType {
  Invalid = "Invalid",
  Land = "Land",
  Water = "Water",
  Tree = "Tree",
}

export const encodeTileType: { [key: string]: number } = {
  Invalid: 0,
  Land: 1,
  Water: 2,
  Tree: 3,
};

export const decodeTileType: { [key: number]: TileType } = {
  0: TileType.Invalid,
  1: TileType.Land,
  2: TileType.Water,
  3: TileType.Tree,
};

export interface SearchForGame {
  ranked?: boolean;
}

export function encodeSearchForGame(message: SearchForGame): Uint8Array {
  let bb = popByteBuffer();
  _encodeSearchForGame(message, bb);
  return toUint8Array(bb);
}

function _encodeSearchForGame(message: SearchForGame, bb: ByteBuffer): void {
  // optional bool ranked = 1;
  let $ranked = message.ranked;
  if ($ranked !== undefined) {
    writeVarint32(bb, 8);
    writeByte(bb, $ranked ? 1 : 0);
  }
}

export function decodeSearchForGame(binary: Uint8Array): SearchForGame {
  return _decodeSearchForGame(wrapByteBuffer(binary));
}

function _decodeSearchForGame(bb: ByteBuffer): SearchForGame {
  let message: SearchForGame = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional bool ranked = 1;
      case 1: {
        message.ranked = !!readByte(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface OneofMatchmakingRequest {
  sender_id?: string;
  search_for_game?: SearchForGame;
}

export function encodeOneofMatchmakingRequest(message: OneofMatchmakingRequest): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneofMatchmakingRequest(message, bb);
  return toUint8Array(bb);
}

function _encodeOneofMatchmakingRequest(message: OneofMatchmakingRequest, bb: ByteBuffer): void {
  // optional string sender_id = 1;
  let $sender_id = message.sender_id;
  if ($sender_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $sender_id);
  }

  // optional SearchForGame search_for_game = 2;
  let $search_for_game = message.search_for_game;
  if ($search_for_game !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeSearchForGame($search_for_game, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneofMatchmakingRequest(binary: Uint8Array): OneofMatchmakingRequest {
  return _decodeOneofMatchmakingRequest(wrapByteBuffer(binary));
}

function _decodeOneofMatchmakingRequest(bb: ByteBuffer): OneofMatchmakingRequest {
  let message: OneofMatchmakingRequest = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string sender_id = 1;
      case 1: {
        message.sender_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional SearchForGame search_for_game = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.search_for_game = _decodeSearchForGame(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface FoundGame {
  game_id?: string;
  server_url?: string;
}

export function encodeFoundGame(message: FoundGame): Uint8Array {
  let bb = popByteBuffer();
  _encodeFoundGame(message, bb);
  return toUint8Array(bb);
}

function _encodeFoundGame(message: FoundGame, bb: ByteBuffer): void {
  // optional string game_id = 1;
  let $game_id = message.game_id;
  if ($game_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $game_id);
  }

  // optional string server_url = 2;
  let $server_url = message.server_url;
  if ($server_url !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $server_url);
  }
}

export function decodeFoundGame(binary: Uint8Array): FoundGame {
  return _decodeFoundGame(wrapByteBuffer(binary));
}

function _decodeFoundGame(bb: ByteBuffer): FoundGame {
  let message: FoundGame = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string game_id = 1;
      case 1: {
        message.game_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional string server_url = 2;
      case 2: {
        message.server_url = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface OneofMatchmakingUpdate {
  recipient_id?: string;
  found_game?: FoundGame;
}

export function encodeOneofMatchmakingUpdate(message: OneofMatchmakingUpdate): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneofMatchmakingUpdate(message, bb);
  return toUint8Array(bb);
}

function _encodeOneofMatchmakingUpdate(message: OneofMatchmakingUpdate, bb: ByteBuffer): void {
  // optional string recipient_id = 1;
  let $recipient_id = message.recipient_id;
  if ($recipient_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $recipient_id);
  }

  // optional FoundGame found_game = 2;
  let $found_game = message.found_game;
  if ($found_game !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeFoundGame($found_game, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneofMatchmakingUpdate(binary: Uint8Array): OneofMatchmakingUpdate {
  return _decodeOneofMatchmakingUpdate(wrapByteBuffer(binary));
}

function _decodeOneofMatchmakingUpdate(bb: ByteBuffer): OneofMatchmakingUpdate {
  let message: OneofMatchmakingUpdate = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string recipient_id = 1;
      case 1: {
        message.recipient_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional FoundGame found_game = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.found_game = _decodeFoundGame(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Hello {
  name?: string;
}

export function encodeHello(message: Hello): Uint8Array {
  let bb = popByteBuffer();
  _encodeHello(message, bb);
  return toUint8Array(bb);
}

function _encodeHello(message: Hello, bb: ByteBuffer): void {
  // optional string name = 1;
  let $name = message.name;
  if ($name !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $name);
  }
}

export function decodeHello(binary: Uint8Array): Hello {
  return _decodeHello(wrapByteBuffer(binary));
}

function _decodeHello(bb: ByteBuffer): Hello {
  let message: Hello = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string name = 1;
      case 1: {
        message.name = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface OneofRequest {
  sender_id?: string;
  hello?: Hello;
}

export function encodeOneofRequest(message: OneofRequest): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneofRequest(message, bb);
  return toUint8Array(bb);
}

function _encodeOneofRequest(message: OneofRequest, bb: ByteBuffer): void {
  // optional string sender_id = 1;
  let $sender_id = message.sender_id;
  if ($sender_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $sender_id);
  }

  // optional Hello hello = 2;
  let $hello = message.hello;
  if ($hello !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeHello($hello, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneofRequest(binary: Uint8Array): OneofRequest {
  return _decodeOneofRequest(wrapByteBuffer(binary));
}

function _decodeOneofRequest(bb: ByteBuffer): OneofRequest {
  let message: OneofRequest = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string sender_id = 1;
      case 1: {
        message.sender_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional Hello hello = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.hello = _decodeHello(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface OneofUpdate {
  recipient_id?: string;
  initial_state?: InitialState;
  all_soldier_positions?: AllSoldierPositions;
}

export function encodeOneofUpdate(message: OneofUpdate): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneofUpdate(message, bb);
  return toUint8Array(bb);
}

function _encodeOneofUpdate(message: OneofUpdate, bb: ByteBuffer): void {
  // optional string recipient_id = 1;
  let $recipient_id = message.recipient_id;
  if ($recipient_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $recipient_id);
  }

  // optional InitialState initial_state = 2;
  let $initial_state = message.initial_state;
  if ($initial_state !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeInitialState($initial_state, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional AllSoldierPositions all_soldier_positions = 3;
  let $all_soldier_positions = message.all_soldier_positions;
  if ($all_soldier_positions !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeAllSoldierPositions($all_soldier_positions, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneofUpdate(binary: Uint8Array): OneofUpdate {
  return _decodeOneofUpdate(wrapByteBuffer(binary));
}

function _decodeOneofUpdate(bb: ByteBuffer): OneofUpdate {
  let message: OneofUpdate = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string recipient_id = 1;
      case 1: {
        message.recipient_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional InitialState initial_state = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.initial_state = _decodeInitialState(bb);
        bb.limit = limit;
        break;
      }

      // optional AllSoldierPositions all_soldier_positions = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.all_soldier_positions = _decodeAllSoldierPositions(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Chunk {
  index?: number;
  maxIndex?: number;
  data?: Uint8Array;
}

export function encodeChunk(message: Chunk): Uint8Array {
  let bb = popByteBuffer();
  _encodeChunk(message, bb);
  return toUint8Array(bb);
}

function _encodeChunk(message: Chunk, bb: ByteBuffer): void {
  // optional int32 index = 1;
  let $index = message.index;
  if ($index !== undefined) {
    writeVarint32(bb, 8);
    writeVarint64(bb, intToLong($index));
  }

  // optional int32 maxIndex = 2;
  let $maxIndex = message.maxIndex;
  if ($maxIndex !== undefined) {
    writeVarint32(bb, 16);
    writeVarint64(bb, intToLong($maxIndex));
  }

  // optional bytes data = 3;
  let $data = message.data;
  if ($data !== undefined) {
    writeVarint32(bb, 26);
    writeVarint32(bb, $data.length), writeBytes(bb, $data);
  }
}

export function decodeChunk(binary: Uint8Array): Chunk {
  return _decodeChunk(wrapByteBuffer(binary));
}

function _decodeChunk(bb: ByteBuffer): Chunk {
  let message: Chunk = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional int32 index = 1;
      case 1: {
        message.index = readVarint32(bb);
        break;
      }

      // optional int32 maxIndex = 2;
      case 2: {
        message.maxIndex = readVarint32(bb);
        break;
      }

      // optional bytes data = 3;
      case 3: {
        message.data = readBytes(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Packet {
  chunks?: Chunk[];
  id?: Long;
  type?: PacketType;
  sent_ms?: number;
}

export function encodePacket(message: Packet): Uint8Array {
  let bb = popByteBuffer();
  _encodePacket(message, bb);
  return toUint8Array(bb);
}

function _encodePacket(message: Packet, bb: ByteBuffer): void {
  // repeated Chunk chunks = 1;
  let array$chunks = message.chunks;
  if (array$chunks !== undefined) {
    for (let value of array$chunks) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeChunk(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }

  // optional uint64 id = 2;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 16);
    writeVarint64(bb, $id);
  }

  // optional PacketType type = 3;
  let $type = message.type;
  if ($type !== undefined) {
    writeVarint32(bb, 24);
    writeVarint32(bb, encodePacketType[$type]);
  }

  // optional int32 sent_ms = 4;
  let $sent_ms = message.sent_ms;
  if ($sent_ms !== undefined) {
    writeVarint32(bb, 32);
    writeVarint64(bb, intToLong($sent_ms));
  }
}

export function decodePacket(binary: Uint8Array): Packet {
  return _decodePacket(wrapByteBuffer(binary));
}

function _decodePacket(bb: ByteBuffer): Packet {
  let message: Packet = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated Chunk chunks = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.chunks || (message.chunks = []);
        values.push(_decodeChunk(bb));
        bb.limit = limit;
        break;
      }

      // optional uint64 id = 2;
      case 2: {
        message.id = readVarint64(bb, /* unsigned */ true);
        break;
      }

      // optional PacketType type = 3;
      case 3: {
        message.type = decodePacketType[readVarint32(bb)];
        break;
      }

      // optional int32 sent_ms = 4;
      case 4: {
        message.sent_ms = readVarint32(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface V2 {
  x?: number;
  y?: number;
}

export function encodeV2(message: V2): Uint8Array {
  let bb = popByteBuffer();
  _encodeV2(message, bb);
  return toUint8Array(bb);
}

function _encodeV2(message: V2, bb: ByteBuffer): void {
  // optional float x = 1;
  let $x = message.x;
  if ($x !== undefined) {
    writeVarint32(bb, 13);
    writeFloat(bb, $x);
  }

  // optional float y = 2;
  let $y = message.y;
  if ($y !== undefined) {
    writeVarint32(bb, 21);
    writeFloat(bb, $y);
  }
}

export function decodeV2(binary: Uint8Array): V2 {
  return _decodeV2(wrapByteBuffer(binary));
}

function _decodeV2(bb: ByteBuffer): V2 {
  let message: V2 = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional float x = 1;
      case 1: {
        message.x = readFloat(bb);
        break;
      }

      // optional float y = 2;
      case 2: {
        message.y = readFloat(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface V2Int {
  x?: number;
  y?: number;
}

export function encodeV2Int(message: V2Int): Uint8Array {
  let bb = popByteBuffer();
  _encodeV2Int(message, bb);
  return toUint8Array(bb);
}

function _encodeV2Int(message: V2Int, bb: ByteBuffer): void {
  // optional int32 x = 1;
  let $x = message.x;
  if ($x !== undefined) {
    writeVarint32(bb, 8);
    writeVarint64(bb, intToLong($x));
  }

  // optional int32 y = 2;
  let $y = message.y;
  if ($y !== undefined) {
    writeVarint32(bb, 16);
    writeVarint64(bb, intToLong($y));
  }
}

export function decodeV2Int(binary: Uint8Array): V2Int {
  return _decodeV2Int(wrapByteBuffer(binary));
}

function _decodeV2Int(bb: ByteBuffer): V2Int {
  let message: V2Int = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional int32 x = 1;
      case 1: {
        message.x = readVarint32(bb);
        break;
      }

      // optional int32 y = 2;
      case 2: {
        message.y = readVarint32(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface AllSoldierPositions {
  soldier_positions?: SoldierPosition[];
}

export function encodeAllSoldierPositions(message: AllSoldierPositions): Uint8Array {
  let bb = popByteBuffer();
  _encodeAllSoldierPositions(message, bb);
  return toUint8Array(bb);
}

function _encodeAllSoldierPositions(message: AllSoldierPositions, bb: ByteBuffer): void {
  // repeated SoldierPosition soldier_positions = 1;
  let array$soldier_positions = message.soldier_positions;
  if (array$soldier_positions !== undefined) {
    for (let value of array$soldier_positions) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeSoldierPosition(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeAllSoldierPositions(binary: Uint8Array): AllSoldierPositions {
  return _decodeAllSoldierPositions(wrapByteBuffer(binary));
}

function _decodeAllSoldierPositions(bb: ByteBuffer): AllSoldierPositions {
  let message: AllSoldierPositions = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated SoldierPosition soldier_positions = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.soldier_positions || (message.soldier_positions = []);
        values.push(_decodeSoldierPosition(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface SoldierPosition {
  id?: Long;
  pos?: V2;
  velocity?: V2;
}

export function encodeSoldierPosition(message: SoldierPosition): Uint8Array {
  let bb = popByteBuffer();
  _encodeSoldierPosition(message, bb);
  return toUint8Array(bb);
}

function _encodeSoldierPosition(message: SoldierPosition, bb: ByteBuffer): void {
  // optional uint64 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint64(bb, $id);
  }

  // optional V2 pos = 2;
  let $pos = message.pos;
  if ($pos !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeV2($pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional V2 velocity = 3;
  let $velocity = message.velocity;
  if ($velocity !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeV2($velocity, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeSoldierPosition(binary: Uint8Array): SoldierPosition {
  return _decodeSoldierPosition(wrapByteBuffer(binary));
}

function _decodeSoldierPosition(bb: ByteBuffer): SoldierPosition {
  let message: SoldierPosition = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint64 id = 1;
      case 1: {
        message.id = readVarint64(bb, /* unsigned */ true);
        break;
      }

      // optional V2 pos = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.pos = _decodeV2(bb);
        bb.limit = limit;
        break;
      }

      // optional V2 velocity = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.velocity = _decodeV2(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface KeepState {
  id?: Long;
  pos?: V2;
  warrior_count?: number;
  archer_count?: number;
  name?: string;
  alliance?: number;
}

export function encodeKeepState(message: KeepState): Uint8Array {
  let bb = popByteBuffer();
  _encodeKeepState(message, bb);
  return toUint8Array(bb);
}

function _encodeKeepState(message: KeepState, bb: ByteBuffer): void {
  // optional uint64 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint64(bb, $id);
  }

  // optional V2 pos = 2;
  let $pos = message.pos;
  if ($pos !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeV2($pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional int32 warrior_count = 3;
  let $warrior_count = message.warrior_count;
  if ($warrior_count !== undefined) {
    writeVarint32(bb, 24);
    writeVarint64(bb, intToLong($warrior_count));
  }

  // optional int32 archer_count = 4;
  let $archer_count = message.archer_count;
  if ($archer_count !== undefined) {
    writeVarint32(bb, 32);
    writeVarint64(bb, intToLong($archer_count));
  }

  // optional string name = 5;
  let $name = message.name;
  if ($name !== undefined) {
    writeVarint32(bb, 42);
    writeString(bb, $name);
  }

  // optional int32 alliance = 6;
  let $alliance = message.alliance;
  if ($alliance !== undefined) {
    writeVarint32(bb, 48);
    writeVarint64(bb, intToLong($alliance));
  }
}

export function decodeKeepState(binary: Uint8Array): KeepState {
  return _decodeKeepState(wrapByteBuffer(binary));
}

function _decodeKeepState(bb: ByteBuffer): KeepState {
  let message: KeepState = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint64 id = 1;
      case 1: {
        message.id = readVarint64(bb, /* unsigned */ true);
        break;
      }

      // optional V2 pos = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.pos = _decodeV2(bb);
        bb.limit = limit;
        break;
      }

      // optional int32 warrior_count = 3;
      case 3: {
        message.warrior_count = readVarint32(bb);
        break;
      }

      // optional int32 archer_count = 4;
      case 4: {
        message.archer_count = readVarint32(bb);
        break;
      }

      // optional string name = 5;
      case 5: {
        message.name = readString(bb, readVarint32(bb));
        break;
      }

      // optional int32 alliance = 6;
      case 6: {
        message.alliance = readVarint32(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface InitialState {
  keeps?: KeepState[];
  map_width?: number;
  map_height?: number;
  tiles?: TileType[];
}

export function encodeInitialState(message: InitialState): Uint8Array {
  let bb = popByteBuffer();
  _encodeInitialState(message, bb);
  return toUint8Array(bb);
}

function _encodeInitialState(message: InitialState, bb: ByteBuffer): void {
  // repeated KeepState keeps = 1;
  let array$keeps = message.keeps;
  if (array$keeps !== undefined) {
    for (let value of array$keeps) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeKeepState(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }

  // optional int32 map_width = 2;
  let $map_width = message.map_width;
  if ($map_width !== undefined) {
    writeVarint32(bb, 16);
    writeVarint64(bb, intToLong($map_width));
  }

  // optional int32 map_height = 3;
  let $map_height = message.map_height;
  if ($map_height !== undefined) {
    writeVarint32(bb, 24);
    writeVarint64(bb, intToLong($map_height));
  }

  // repeated TileType tiles = 4;
  let array$tiles = message.tiles;
  if (array$tiles !== undefined) {
    let packed = popByteBuffer();
    for (let value of array$tiles) {
      writeVarint32(packed, encodeTileType[value]);
    }
    writeVarint32(bb, 34);
    writeVarint32(bb, packed.offset);
    writeByteBuffer(bb, packed);
    pushByteBuffer(packed);
  }
}

export function decodeInitialState(binary: Uint8Array): InitialState {
  return _decodeInitialState(wrapByteBuffer(binary));
}

function _decodeInitialState(bb: ByteBuffer): InitialState {
  let message: InitialState = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated KeepState keeps = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.keeps || (message.keeps = []);
        values.push(_decodeKeepState(bb));
        bb.limit = limit;
        break;
      }

      // optional int32 map_width = 2;
      case 2: {
        message.map_width = readVarint32(bb);
        break;
      }

      // optional int32 map_height = 3;
      case 3: {
        message.map_height = readVarint32(bb);
        break;
      }

      // repeated TileType tiles = 4;
      case 4: {
        let values = message.tiles || (message.tiles = []);
        if ((tag & 7) === 2) {
          let outerLimit = pushTemporaryLength(bb);
          while (!isAtEnd(bb)) {
            values.push(decodeTileType[readVarint32(bb)]);
          }
          bb.limit = outerLimit;
        } else {
          values.push(decodeTileType[readVarint32(bb)]);
        }
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Long {
  low: number;
  high: number;
  unsigned: boolean;
}

interface ByteBuffer {
  bytes: Uint8Array;
  offset: number;
  limit: number;
}

function pushTemporaryLength(bb: ByteBuffer): number {
  let length = readVarint32(bb);
  let limit = bb.limit;
  bb.limit = bb.offset + length;
  return limit;
}

function skipUnknownField(bb: ByteBuffer, type: number): void {
  switch (type) {
    case 0: while (readByte(bb) & 0x80) { } break;
    case 2: skip(bb, readVarint32(bb)); break;
    case 5: skip(bb, 4); break;
    case 1: skip(bb, 8); break;
    default: throw new Error("Unimplemented type: " + type);
  }
}

function stringToLong(value: string): Long {
  return {
    low: value.charCodeAt(0) | (value.charCodeAt(1) << 16),
    high: value.charCodeAt(2) | (value.charCodeAt(3) << 16),
    unsigned: false,
  };
}

function longToString(value: Long): string {
  let low = value.low;
  let high = value.high;
  return String.fromCharCode(
    low & 0xFFFF,
    low >>> 16,
    high & 0xFFFF,
    high >>> 16);
}

// The code below was modified from https://github.com/protobufjs/bytebuffer.js
// which is under the Apache License 2.0.

let f32 = new Float32Array(1);
let f32_u8 = new Uint8Array(f32.buffer);

let f64 = new Float64Array(1);
let f64_u8 = new Uint8Array(f64.buffer);

function intToLong(value: number): Long {
  value |= 0;
  return {
    low: value,
    high: value >> 31,
    unsigned: value >= 0,
  };
}

let bbStack: ByteBuffer[] = [];

function popByteBuffer(): ByteBuffer {
  const bb = bbStack.pop();
  if (!bb) return { bytes: new Uint8Array(64), offset: 0, limit: 0 };
  bb.offset = bb.limit = 0;
  return bb;
}

function pushByteBuffer(bb: ByteBuffer): void {
  bbStack.push(bb);
}

function wrapByteBuffer(bytes: Uint8Array): ByteBuffer {
  return { bytes, offset: 0, limit: bytes.length };
}

function toUint8Array(bb: ByteBuffer): Uint8Array {
  let bytes = bb.bytes;
  let limit = bb.limit;
  return bytes.length === limit ? bytes : bytes.subarray(0, limit);
}

function skip(bb: ByteBuffer, offset: number): void {
  if (bb.offset + offset > bb.limit) {
    throw new Error('Skip past limit');
  }
  bb.offset += offset;
}

function isAtEnd(bb: ByteBuffer): boolean {
  return bb.offset >= bb.limit;
}

function grow(bb: ByteBuffer, count: number): number {
  let bytes = bb.bytes;
  let offset = bb.offset;
  let limit = bb.limit;
  let finalOffset = offset + count;
  if (finalOffset > bytes.length) {
    let newBytes = new Uint8Array(finalOffset * 2);
    newBytes.set(bytes);
    bb.bytes = newBytes;
  }
  bb.offset = finalOffset;
  if (finalOffset > limit) {
    bb.limit = finalOffset;
  }
  return offset;
}

function advance(bb: ByteBuffer, count: number): number {
  let offset = bb.offset;
  if (offset + count > bb.limit) {
    throw new Error('Read past limit');
  }
  bb.offset += count;
  return offset;
}

function readBytes(bb: ByteBuffer, count: number): Uint8Array {
  let offset = advance(bb, count);
  return bb.bytes.subarray(offset, offset + count);
}

function writeBytes(bb: ByteBuffer, buffer: Uint8Array): void {
  let offset = grow(bb, buffer.length);
  bb.bytes.set(buffer, offset);
}

function readString(bb: ByteBuffer, count: number): string {
  // Sadly a hand-coded UTF8 decoder is much faster than subarray+TextDecoder in V8
  let offset = advance(bb, count);
  let fromCharCode = String.fromCharCode;
  let bytes = bb.bytes;
  let invalid = '\uFFFD';
  let text = '';

  for (let i = 0; i < count; i++) {
    let c1 = bytes[i + offset], c2: number, c3: number, c4: number, c: number;

    // 1 byte
    if ((c1 & 0x80) === 0) {
      text += fromCharCode(c1);
    }

    // 2 bytes
    else if ((c1 & 0xE0) === 0xC0) {
      if (i + 1 >= count) text += invalid;
      else {
        c2 = bytes[i + offset + 1];
        if ((c2 & 0xC0) !== 0x80) text += invalid;
        else {
          c = ((c1 & 0x1F) << 6) | (c2 & 0x3F);
          if (c < 0x80) text += invalid;
          else {
            text += fromCharCode(c);
            i++;
          }
        }
      }
    }

    // 3 bytes
    else if ((c1 & 0xF0) == 0xE0) {
      if (i + 2 >= count) text += invalid;
      else {
        c2 = bytes[i + offset + 1];
        c3 = bytes[i + offset + 2];
        if (((c2 | (c3 << 8)) & 0xC0C0) !== 0x8080) text += invalid;
        else {
          c = ((c1 & 0x0F) << 12) | ((c2 & 0x3F) << 6) | (c3 & 0x3F);
          if (c < 0x0800 || (c >= 0xD800 && c <= 0xDFFF)) text += invalid;
          else {
            text += fromCharCode(c);
            i += 2;
          }
        }
      }
    }

    // 4 bytes
    else if ((c1 & 0xF8) == 0xF0) {
      if (i + 3 >= count) text += invalid;
      else {
        c2 = bytes[i + offset + 1];
        c3 = bytes[i + offset + 2];
        c4 = bytes[i + offset + 3];
        if (((c2 | (c3 << 8) | (c4 << 16)) & 0xC0C0C0) !== 0x808080) text += invalid;
        else {
          c = ((c1 & 0x07) << 0x12) | ((c2 & 0x3F) << 0x0C) | ((c3 & 0x3F) << 0x06) | (c4 & 0x3F);
          if (c < 0x10000 || c > 0x10FFFF) text += invalid;
          else {
            c -= 0x10000;
            text += fromCharCode((c >> 10) + 0xD800, (c & 0x3FF) + 0xDC00);
            i += 3;
          }
        }
      }
    }

    else text += invalid;
  }

  return text;
}

function writeString(bb: ByteBuffer, text: string): void {
  // Sadly a hand-coded UTF8 encoder is much faster than TextEncoder+set in V8
  let n = text.length;
  let byteCount = 0;

  // Write the byte count first
  for (let i = 0; i < n; i++) {
    let c = text.charCodeAt(i);
    if (c >= 0xD800 && c <= 0xDBFF && i + 1 < n) {
      c = (c << 10) + text.charCodeAt(++i) - 0x35FDC00;
    }
    byteCount += c < 0x80 ? 1 : c < 0x800 ? 2 : c < 0x10000 ? 3 : 4;
  }
  writeVarint32(bb, byteCount);

  let offset = grow(bb, byteCount);
  let bytes = bb.bytes;

  // Then write the bytes
  for (let i = 0; i < n; i++) {
    let c = text.charCodeAt(i);
    if (c >= 0xD800 && c <= 0xDBFF && i + 1 < n) {
      c = (c << 10) + text.charCodeAt(++i) - 0x35FDC00;
    }
    if (c < 0x80) {
      bytes[offset++] = c;
    } else {
      if (c < 0x800) {
        bytes[offset++] = ((c >> 6) & 0x1F) | 0xC0;
      } else {
        if (c < 0x10000) {
          bytes[offset++] = ((c >> 12) & 0x0F) | 0xE0;
        } else {
          bytes[offset++] = ((c >> 18) & 0x07) | 0xF0;
          bytes[offset++] = ((c >> 12) & 0x3F) | 0x80;
        }
        bytes[offset++] = ((c >> 6) & 0x3F) | 0x80;
      }
      bytes[offset++] = (c & 0x3F) | 0x80;
    }
  }
}

function writeByteBuffer(bb: ByteBuffer, buffer: ByteBuffer): void {
  let offset = grow(bb, buffer.limit);
  let from = bb.bytes;
  let to = buffer.bytes;

  // This for loop is much faster than subarray+set on V8
  for (let i = 0, n = buffer.limit; i < n; i++) {
    from[i + offset] = to[i];
  }
}

function readByte(bb: ByteBuffer): number {
  return bb.bytes[advance(bb, 1)];
}

function writeByte(bb: ByteBuffer, value: number): void {
  let offset = grow(bb, 1);
  bb.bytes[offset] = value;
}

function readFloat(bb: ByteBuffer): number {
  let offset = advance(bb, 4);
  let bytes = bb.bytes;

  // Manual copying is much faster than subarray+set in V8
  f32_u8[0] = bytes[offset++];
  f32_u8[1] = bytes[offset++];
  f32_u8[2] = bytes[offset++];
  f32_u8[3] = bytes[offset++];
  return f32[0];
}

function writeFloat(bb: ByteBuffer, value: number): void {
  let offset = grow(bb, 4);
  let bytes = bb.bytes;
  f32[0] = value;

  // Manual copying is much faster than subarray+set in V8
  bytes[offset++] = f32_u8[0];
  bytes[offset++] = f32_u8[1];
  bytes[offset++] = f32_u8[2];
  bytes[offset++] = f32_u8[3];
}

function readDouble(bb: ByteBuffer): number {
  let offset = advance(bb, 8);
  let bytes = bb.bytes;

  // Manual copying is much faster than subarray+set in V8
  f64_u8[0] = bytes[offset++];
  f64_u8[1] = bytes[offset++];
  f64_u8[2] = bytes[offset++];
  f64_u8[3] = bytes[offset++];
  f64_u8[4] = bytes[offset++];
  f64_u8[5] = bytes[offset++];
  f64_u8[6] = bytes[offset++];
  f64_u8[7] = bytes[offset++];
  return f64[0];
}

function writeDouble(bb: ByteBuffer, value: number): void {
  let offset = grow(bb, 8);
  let bytes = bb.bytes;
  f64[0] = value;

  // Manual copying is much faster than subarray+set in V8
  bytes[offset++] = f64_u8[0];
  bytes[offset++] = f64_u8[1];
  bytes[offset++] = f64_u8[2];
  bytes[offset++] = f64_u8[3];
  bytes[offset++] = f64_u8[4];
  bytes[offset++] = f64_u8[5];
  bytes[offset++] = f64_u8[6];
  bytes[offset++] = f64_u8[7];
}

function readInt32(bb: ByteBuffer): number {
  let offset = advance(bb, 4);
  let bytes = bb.bytes;
  return (
    bytes[offset] |
    (bytes[offset + 1] << 8) |
    (bytes[offset + 2] << 16) |
    (bytes[offset + 3] << 24)
  );
}

function writeInt32(bb: ByteBuffer, value: number): void {
  let offset = grow(bb, 4);
  let bytes = bb.bytes;
  bytes[offset] = value;
  bytes[offset + 1] = value >> 8;
  bytes[offset + 2] = value >> 16;
  bytes[offset + 3] = value >> 24;
}

function readInt64(bb: ByteBuffer, unsigned: boolean): Long {
  return {
    low: readInt32(bb),
    high: readInt32(bb),
    unsigned,
  };
}

function writeInt64(bb: ByteBuffer, value: Long): void {
  writeInt32(bb, value.low);
  writeInt32(bb, value.high);
}

function readVarint32(bb: ByteBuffer): number {
  let c = 0;
  let value = 0;
  let b: number;
  do {
    b = readByte(bb);
    if (c < 32) value |= (b & 0x7F) << c;
    c += 7;
  } while (b & 0x80);
  return value;
}

function writeVarint32(bb: ByteBuffer, value: number): void {
  value >>>= 0;
  while (value >= 0x80) {
    writeByte(bb, (value & 0x7f) | 0x80);
    value >>>= 7;
  }
  writeByte(bb, value);
}

function readVarint64(bb: ByteBuffer, unsigned: boolean): Long {
  let part0 = 0;
  let part1 = 0;
  let part2 = 0;
  let b: number;

  b = readByte(bb); part0 = (b & 0x7F); if (b & 0x80) {
    b = readByte(bb); part0 |= (b & 0x7F) << 7; if (b & 0x80) {
      b = readByte(bb); part0 |= (b & 0x7F) << 14; if (b & 0x80) {
        b = readByte(bb); part0 |= (b & 0x7F) << 21; if (b & 0x80) {

          b = readByte(bb); part1 = (b & 0x7F); if (b & 0x80) {
            b = readByte(bb); part1 |= (b & 0x7F) << 7; if (b & 0x80) {
              b = readByte(bb); part1 |= (b & 0x7F) << 14; if (b & 0x80) {
                b = readByte(bb); part1 |= (b & 0x7F) << 21; if (b & 0x80) {

                  b = readByte(bb); part2 = (b & 0x7F); if (b & 0x80) {
                    b = readByte(bb); part2 |= (b & 0x7F) << 7;
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  return {
    low: part0 | (part1 << 28),
    high: (part1 >>> 4) | (part2 << 24),
    unsigned,
  };
}

function writeVarint64(bb: ByteBuffer, value: Long): void {
  let part0 = value.low >>> 0;
  let part1 = ((value.low >>> 28) | (value.high << 4)) >>> 0;
  let part2 = value.high >>> 24;

  // ref: src/google/protobuf/io/coded_stream.cc
  let size =
    part2 === 0 ?
      part1 === 0 ?
        part0 < 1 << 14 ?
          part0 < 1 << 7 ? 1 : 2 :
          part0 < 1 << 21 ? 3 : 4 :
        part1 < 1 << 14 ?
          part1 < 1 << 7 ? 5 : 6 :
          part1 < 1 << 21 ? 7 : 8 :
      part2 < 1 << 7 ? 9 : 10;

  let offset = grow(bb, size);
  let bytes = bb.bytes;

  switch (size) {
    case 10: bytes[offset + 9] = (part2 >>> 7) & 0x01;
    case 9: bytes[offset + 8] = size !== 9 ? part2 | 0x80 : part2 & 0x7F;
    case 8: bytes[offset + 7] = size !== 8 ? (part1 >>> 21) | 0x80 : (part1 >>> 21) & 0x7F;
    case 7: bytes[offset + 6] = size !== 7 ? (part1 >>> 14) | 0x80 : (part1 >>> 14) & 0x7F;
    case 6: bytes[offset + 5] = size !== 6 ? (part1 >>> 7) | 0x80 : (part1 >>> 7) & 0x7F;
    case 5: bytes[offset + 4] = size !== 5 ? part1 | 0x80 : part1 & 0x7F;
    case 4: bytes[offset + 3] = size !== 4 ? (part0 >>> 21) | 0x80 : (part0 >>> 21) & 0x7F;
    case 3: bytes[offset + 2] = size !== 3 ? (part0 >>> 14) | 0x80 : (part0 >>> 14) & 0x7F;
    case 2: bytes[offset + 1] = size !== 2 ? (part0 >>> 7) | 0x80 : (part0 >>> 7) & 0x7F;
    case 1: bytes[offset] = size !== 1 ? part0 | 0x80 : part0 & 0x7F;
  }
}

function readVarint32ZigZag(bb: ByteBuffer): number {
  let value = readVarint32(bb);

  // ref: src/google/protobuf/wire_format_lite.h
  return (value >>> 1) ^ -(value & 1);
}

function writeVarint32ZigZag(bb: ByteBuffer, value: number): void {
  // ref: src/google/protobuf/wire_format_lite.h
  writeVarint32(bb, (value << 1) ^ (value >> 31));
}

function readVarint64ZigZag(bb: ByteBuffer): Long {
  let value = readVarint64(bb, /* unsigned */ false);
  let low = value.low;
  let high = value.high;
  let flip = -(low & 1);

  // ref: src/google/protobuf/wire_format_lite.h
  return {
    low: ((low >>> 1) | (high << 31)) ^ flip,
    high: (high >>> 1) ^ flip,
    unsigned: false,
  };
}

function writeVarint64ZigZag(bb: ByteBuffer, value: Long): void {
  let low = value.low;
  let high = value.high;
  let flip = high >> 31;

  // ref: src/google/protobuf/wire_format_lite.h
  writeVarint64(bb, {
    low: (low << 1) ^ flip,
    high: ((high << 1) | (low >>> 31)) ^ flip,
    unsigned: false,
  });
}
