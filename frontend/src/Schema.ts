export const enum GenerationMode {
  AutoAccrue = "AutoAccrue",
  Word = "Word",
}

export const encodeGenerationMode: { [key: string]: number } = {
  AutoAccrue: 0,
  Word: 1,
};

export const decodeGenerationMode: { [key: number]: GenerationMode } = {
  0: GenerationMode.AutoAccrue,
  1: GenerationMode.Word,
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

export const enum RenderAllianceCase {
  InvalidRenderAlliance = "InvalidRenderAlliance",
  FullLand_SingleRoundedCorner = "FullLand_SingleRoundedCorner",
  FullLand_SplitDownMiddle = "FullLand_SplitDownMiddle",
  FullLand_IndividualCorners = "FullLand_IndividualCorners",
  FullLand_OneOwner = "FullLand_OneOwner",
  ThreeCorners_OneOwner = "ThreeCorners_OneOwner",
  ThreeCorners_TwoOwners = "ThreeCorners_TwoOwners",
  ThreeCorners_ThreeOwners = "ThreeCorners_ThreeOwners",
  TwoAdjacent_TwoOwners = "TwoAdjacent_TwoOwners",
  TwoAdjacent_OneOwner = "TwoAdjacent_OneOwner",
  TwoOpposite_TwoOwners = "TwoOpposite_TwoOwners",
  TwoOpposite_OneOwner = "TwoOpposite_OneOwner",
  SingleCorner_OneOwner = "SingleCorner_OneOwner",
  FullWater_NoOnwer = "FullWater_NoOnwer",
}

export const encodeRenderAllianceCase: { [key: string]: number } = {
  InvalidRenderAlliance: 0,
  FullLand_SingleRoundedCorner: 1,
  FullLand_SplitDownMiddle: 2,
  FullLand_IndividualCorners: 3,
  FullLand_OneOwner: 4,
  ThreeCorners_OneOwner: 5,
  ThreeCorners_TwoOwners: 6,
  ThreeCorners_ThreeOwners: 7,
  TwoAdjacent_TwoOwners: 8,
  TwoAdjacent_OneOwner: 9,
  TwoOpposite_TwoOwners: 10,
  TwoOpposite_OneOwner: 11,
  SingleCorner_OneOwner: 12,
  FullWater_NoOnwer: 13,
};

export const decodeRenderAllianceCase: { [key: number]: RenderAllianceCase } = {
  0: RenderAllianceCase.InvalidRenderAlliance,
  1: RenderAllianceCase.FullLand_SingleRoundedCorner,
  2: RenderAllianceCase.FullLand_SplitDownMiddle,
  3: RenderAllianceCase.FullLand_IndividualCorners,
  4: RenderAllianceCase.FullLand_OneOwner,
  5: RenderAllianceCase.ThreeCorners_OneOwner,
  6: RenderAllianceCase.ThreeCorners_TwoOwners,
  7: RenderAllianceCase.ThreeCorners_ThreeOwners,
  8: RenderAllianceCase.TwoAdjacent_TwoOwners,
  9: RenderAllianceCase.TwoAdjacent_OneOwner,
  10: RenderAllianceCase.TwoOpposite_TwoOwners,
  11: RenderAllianceCase.TwoOpposite_OneOwner,
  12: RenderAllianceCase.SingleCorner_OneOwner,
  13: RenderAllianceCase.FullWater_NoOnwer,
};

export const enum SoldierType {
  InvalidSoldier = "InvalidSoldier",
  Warrior = "Warrior",
  Archer = "Archer",
}

export const encodeSoldierType: { [key: string]: number } = {
  InvalidSoldier: 0,
  Warrior: 1,
  Archer: 2,
};

export const decodeSoldierType: { [key: number]: SoldierType } = {
  0: SoldierType.InvalidSoldier,
  1: SoldierType.Warrior,
  2: SoldierType.Archer,
};

export const enum WalkPathType {
  StraightToRight = "StraightToRight",
  StraightDown = "StraightDown",
  StraightLeft = "StraightLeft",
  StraightUp = "StraightUp",
  CircularLeftDown = "CircularLeftDown",
  CircularLeftUp = "CircularLeftUp",
  CircularDownLeft = "CircularDownLeft",
  CircularDownRight = "CircularDownRight",
  CircularRightDown = "CircularRightDown",
  CircularRightUp = "CircularRightUp",
  CircularUpRight = "CircularUpRight",
  CircularUpLeft = "CircularUpLeft",
}

export const encodeWalkPathType: { [key: string]: number } = {
  StraightToRight: 0,
  StraightDown: 1,
  StraightLeft: 2,
  StraightUp: 3,
  CircularLeftDown: 4,
  CircularLeftUp: 5,
  CircularDownLeft: 6,
  CircularDownRight: 7,
  CircularRightDown: 8,
  CircularRightUp: 9,
  CircularUpRight: 10,
  CircularUpLeft: 11,
};

export const decodeWalkPathType: { [key: number]: WalkPathType } = {
  0: WalkPathType.StraightToRight,
  1: WalkPathType.StraightDown,
  2: WalkPathType.StraightLeft,
  3: WalkPathType.StraightUp,
  4: WalkPathType.CircularLeftDown,
  5: WalkPathType.CircularLeftUp,
  6: WalkPathType.CircularDownLeft,
  7: WalkPathType.CircularDownRight,
  8: WalkPathType.CircularRightDown,
  9: WalkPathType.CircularRightUp,
  10: WalkPathType.CircularUpRight,
  11: WalkPathType.CircularUpLeft,
};

export interface GameSettings {
  generation_mode?: GenerationMode;
  map?: string;
}

export function encodeGameSettings(message: GameSettings): Uint8Array {
  let bb = popByteBuffer();
  _encodeGameSettings(message, bb);
  return toUint8Array(bb);
}

function _encodeGameSettings(message: GameSettings, bb: ByteBuffer): void {
  // optional GenerationMode generation_mode = 1;
  let $generation_mode = message.generation_mode;
  if ($generation_mode !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, encodeGenerationMode[$generation_mode]);
  }

  // optional string map = 2;
  let $map = message.map;
  if ($map !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $map);
  }
}

export function decodeGameSettings(binary: Uint8Array): GameSettings {
  return _decodeGameSettings(wrapByteBuffer(binary));
}

function _decodeGameSettings(bb: ByteBuffer): GameSettings {
  let message: GameSettings = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional GenerationMode generation_mode = 1;
      case 1: {
        message.generation_mode = decodeGenerationMode[readVarint32(bb)];
        break;
      }

      // optional string map = 2;
      case 2: {
        message.map = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

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

export interface GameAvailableOnPort {
  game_id?: string;
  player_id?: string;
  port?: string;
}

export function encodeGameAvailableOnPort(message: GameAvailableOnPort): Uint8Array {
  let bb = popByteBuffer();
  _encodeGameAvailableOnPort(message, bb);
  return toUint8Array(bb);
}

function _encodeGameAvailableOnPort(message: GameAvailableOnPort, bb: ByteBuffer): void {
  // optional string game_id = 1;
  let $game_id = message.game_id;
  if ($game_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $game_id);
  }

  // optional string player_id = 2;
  let $player_id = message.player_id;
  if ($player_id !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $player_id);
  }

  // optional string port = 3;
  let $port = message.port;
  if ($port !== undefined) {
    writeVarint32(bb, 26);
    writeString(bb, $port);
  }
}

export function decodeGameAvailableOnPort(binary: Uint8Array): GameAvailableOnPort {
  return _decodeGameAvailableOnPort(wrapByteBuffer(binary));
}

function _decodeGameAvailableOnPort(bb: ByteBuffer): GameAvailableOnPort {
  let message: GameAvailableOnPort = {} as any;

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

      // optional string player_id = 2;
      case 2: {
        message.player_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional string port = 3;
      case 3: {
        message.port = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface GameFoundForPlayer {
  game_id?: string;
  player_id?: string;
  auth_token?: string;
  address?: string;
}

export function encodeGameFoundForPlayer(message: GameFoundForPlayer): Uint8Array {
  let bb = popByteBuffer();
  _encodeGameFoundForPlayer(message, bb);
  return toUint8Array(bb);
}

function _encodeGameFoundForPlayer(message: GameFoundForPlayer, bb: ByteBuffer): void {
  // optional string game_id = 1;
  let $game_id = message.game_id;
  if ($game_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $game_id);
  }

  // optional string player_id = 2;
  let $player_id = message.player_id;
  if ($player_id !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $player_id);
  }

  // optional string auth_token = 3;
  let $auth_token = message.auth_token;
  if ($auth_token !== undefined) {
    writeVarint32(bb, 26);
    writeString(bb, $auth_token);
  }

  // optional string address = 4;
  let $address = message.address;
  if ($address !== undefined) {
    writeVarint32(bb, 34);
    writeString(bb, $address);
  }
}

export function decodeGameFoundForPlayer(binary: Uint8Array): GameFoundForPlayer {
  return _decodeGameFoundForPlayer(wrapByteBuffer(binary));
}

function _decodeGameFoundForPlayer(bb: ByteBuffer): GameFoundForPlayer {
  let message: GameFoundForPlayer = {} as any;

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

      // optional string player_id = 2;
      case 2: {
        message.player_id = readString(bb, readVarint32(bb));
        break;
      }

      // optional string auth_token = 3;
      case 3: {
        message.auth_token = readString(bb, readVarint32(bb));
        break;
      }

      // optional string address = 4;
      case 4: {
        message.address = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface PlacePlayerInGame {
  player_id?: string;
}

export function encodePlacePlayerInGame(message: PlacePlayerInGame): Uint8Array {
  let bb = popByteBuffer();
  _encodePlacePlayerInGame(message, bb);
  return toUint8Array(bb);
}

function _encodePlacePlayerInGame(message: PlacePlayerInGame, bb: ByteBuffer): void {
  // optional string player_id = 1;
  let $player_id = message.player_id;
  if ($player_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $player_id);
  }
}

export function decodePlacePlayerInGame(binary: Uint8Array): PlacePlayerInGame {
  return _decodePlacePlayerInGame(wrapByteBuffer(binary));
}

function _decodePlacePlayerInGame(bb: ByteBuffer): PlacePlayerInGame {
  let message: PlacePlayerInGame = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string player_id = 1;
      case 1: {
        message.player_id = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Registered {
  port?: string;
}

export function encodeRegistered(message: Registered): Uint8Array {
  let bb = popByteBuffer();
  _encodeRegistered(message, bb);
  return toUint8Array(bb);
}

function _encodeRegistered(message: Registered, bb: ByteBuffer): void {
  // optional string port = 1;
  let $port = message.port;
  if ($port !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $port);
  }
}

export function decodeRegistered(binary: Uint8Array): Registered {
  return _decodeRegistered(wrapByteBuffer(binary));
}

function _decodeRegistered(bb: ByteBuffer): Registered {
  let message: Registered = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string port = 1;
      case 1: {
        message.port = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Register {
  port?: string;
}

export function encodeRegister(message: Register): Uint8Array {
  let bb = popByteBuffer();
  _encodeRegister(message, bb);
  return toUint8Array(bb);
}

function _encodeRegister(message: Register, bb: ByteBuffer): void {
  // optional string port = 1;
  let $port = message.port;
  if ($port !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $port);
  }
}

export function decodeRegister(binary: Uint8Array): Register {
  return _decodeRegister(wrapByteBuffer(binary));
}

function _decodeRegister(bb: ByteBuffer): Register {
  let message: Register = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string port = 1;
      case 1: {
        message.port = readString(bb, readVarint32(bb));
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_PlayerToMatchmaker {
  player_id?: string;
  search_for_game?: SearchForGame;
}

export function encodeOneof_PlayerToMatchmaker(message: Oneof_PlayerToMatchmaker): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_PlayerToMatchmaker(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_PlayerToMatchmaker(message: Oneof_PlayerToMatchmaker, bb: ByteBuffer): void {
  // optional string player_id = 1;
  let $player_id = message.player_id;
  if ($player_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $player_id);
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

export function decodeOneof_PlayerToMatchmaker(binary: Uint8Array): Oneof_PlayerToMatchmaker {
  return _decodeOneof_PlayerToMatchmaker(wrapByteBuffer(binary));
}

function _decodeOneof_PlayerToMatchmaker(bb: ByteBuffer): Oneof_PlayerToMatchmaker {
  let message: Oneof_PlayerToMatchmaker = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional string player_id = 1;
      case 1: {
        message.player_id = readString(bb, readVarint32(bb));
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

export interface Oneof_MatchMakerToPlayer {
  found_game?: GameFoundForPlayer;
}

export function encodeOneof_MatchMakerToPlayer(message: Oneof_MatchMakerToPlayer): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_MatchMakerToPlayer(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_MatchMakerToPlayer(message: Oneof_MatchMakerToPlayer, bb: ByteBuffer): void {
  // optional GameFoundForPlayer found_game = 1;
  let $found_game = message.found_game;
  if ($found_game !== undefined) {
    writeVarint32(bb, 10);
    let nested = popByteBuffer();
    _encodeGameFoundForPlayer($found_game, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_MatchMakerToPlayer(binary: Uint8Array): Oneof_MatchMakerToPlayer {
  return _decodeOneof_MatchMakerToPlayer(wrapByteBuffer(binary));
}

function _decodeOneof_MatchMakerToPlayer(bb: ByteBuffer): Oneof_MatchMakerToPlayer {
  let message: Oneof_MatchMakerToPlayer = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional GameFoundForPlayer found_game = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        message.found_game = _decodeGameFoundForPlayer(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_HostServerToMatchmaker {
  game_available_on_port?: GameAvailableOnPort;
  register?: Register;
}

export function encodeOneof_HostServerToMatchmaker(message: Oneof_HostServerToMatchmaker): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_HostServerToMatchmaker(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_HostServerToMatchmaker(message: Oneof_HostServerToMatchmaker, bb: ByteBuffer): void {
  // optional GameAvailableOnPort game_available_on_port = 1;
  let $game_available_on_port = message.game_available_on_port;
  if ($game_available_on_port !== undefined) {
    writeVarint32(bb, 10);
    let nested = popByteBuffer();
    _encodeGameAvailableOnPort($game_available_on_port, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional Register register = 2;
  let $register = message.register;
  if ($register !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeRegister($register, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_HostServerToMatchmaker(binary: Uint8Array): Oneof_HostServerToMatchmaker {
  return _decodeOneof_HostServerToMatchmaker(wrapByteBuffer(binary));
}

function _decodeOneof_HostServerToMatchmaker(bb: ByteBuffer): Oneof_HostServerToMatchmaker {
  let message: Oneof_HostServerToMatchmaker = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional GameAvailableOnPort game_available_on_port = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        message.game_available_on_port = _decodeGameAvailableOnPort(bb);
        bb.limit = limit;
        break;
      }

      // optional Register register = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.register = _decodeRegister(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_MatchmakerToHostServer {
  place_player_in_game?: PlacePlayerInGame;
  registered?: Registered;
}

export function encodeOneof_MatchmakerToHostServer(message: Oneof_MatchmakerToHostServer): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_MatchmakerToHostServer(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_MatchmakerToHostServer(message: Oneof_MatchmakerToHostServer, bb: ByteBuffer): void {
  // optional PlacePlayerInGame place_player_in_game = 2;
  let $place_player_in_game = message.place_player_in_game;
  if ($place_player_in_game !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodePlacePlayerInGame($place_player_in_game, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional Registered registered = 3;
  let $registered = message.registered;
  if ($registered !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeRegistered($registered, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_MatchmakerToHostServer(binary: Uint8Array): Oneof_MatchmakerToHostServer {
  return _decodeOneof_MatchmakerToHostServer(wrapByteBuffer(binary));
}

function _decodeOneof_MatchmakerToHostServer(bb: ByteBuffer): Oneof_MatchmakerToHostServer {
  let message: Oneof_MatchmakerToHostServer = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional PlacePlayerInGame place_player_in_game = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.place_player_in_game = _decodePlacePlayerInGame(bb);
        bb.limit = limit;
        break;
      }

      // optional Registered registered = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.registered = _decodeRegistered(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_HostServerToGameServer {
  register?: Register;
}

export function encodeOneof_HostServerToGameServer(message: Oneof_HostServerToGameServer): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_HostServerToGameServer(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_HostServerToGameServer(message: Oneof_HostServerToGameServer, bb: ByteBuffer): void {
  // optional Register register = 1;
  let $register = message.register;
  if ($register !== undefined) {
    writeVarint32(bb, 10);
    let nested = popByteBuffer();
    _encodeRegister($register, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_HostServerToGameServer(binary: Uint8Array): Oneof_HostServerToGameServer {
  return _decodeOneof_HostServerToGameServer(wrapByteBuffer(binary));
}

function _decodeOneof_HostServerToGameServer(bb: ByteBuffer): Oneof_HostServerToGameServer {
  let message: Oneof_HostServerToGameServer = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional Register register = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        message.register = _decodeRegister(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_PlayerToGameServer {
  sender_id?: string;
  auth_token?: string;
  issue_deployment_order?: IssueDeploymentOrder;
}

export function encodeOneof_PlayerToGameServer(message: Oneof_PlayerToGameServer): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_PlayerToGameServer(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_PlayerToGameServer(message: Oneof_PlayerToGameServer, bb: ByteBuffer): void {
  // optional string sender_id = 1;
  let $sender_id = message.sender_id;
  if ($sender_id !== undefined) {
    writeVarint32(bb, 10);
    writeString(bb, $sender_id);
  }

  // optional string auth_token = 2;
  let $auth_token = message.auth_token;
  if ($auth_token !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $auth_token);
  }

  // optional IssueDeploymentOrder issue_deployment_order = 3;
  let $issue_deployment_order = message.issue_deployment_order;
  if ($issue_deployment_order !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeIssueDeploymentOrder($issue_deployment_order, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_PlayerToGameServer(binary: Uint8Array): Oneof_PlayerToGameServer {
  return _decodeOneof_PlayerToGameServer(wrapByteBuffer(binary));
}

function _decodeOneof_PlayerToGameServer(bb: ByteBuffer): Oneof_PlayerToGameServer {
  let message: Oneof_PlayerToGameServer = {} as any;

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

      // optional string auth_token = 2;
      case 2: {
        message.auth_token = readString(bb, readVarint32(bb));
        break;
      }

      // optional IssueDeploymentOrder issue_deployment_order = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.issue_deployment_order = _decodeIssueDeploymentOrder(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface Oneof_GameServerToPlayer {
  recipient_id?: string;
  initial_state?: InitialState;
  new_soldiers?: NewSoldiers;
  removed_soldiers?: RemovedSoldiers;
  keep_updates?: AllKeepUpdates;
  new_projectiles?: NewProjectiles;
  render_tile_updates?: RenderTileUpdates;
  new_words?: NewWords;
}

export function encodeOneof_GameServerToPlayer(message: Oneof_GameServerToPlayer): Uint8Array {
  let bb = popByteBuffer();
  _encodeOneof_GameServerToPlayer(message, bb);
  return toUint8Array(bb);
}

function _encodeOneof_GameServerToPlayer(message: Oneof_GameServerToPlayer, bb: ByteBuffer): void {
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

  // optional NewSoldiers new_soldiers = 3;
  let $new_soldiers = message.new_soldiers;
  if ($new_soldiers !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeNewSoldiers($new_soldiers, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional RemovedSoldiers removed_soldiers = 4;
  let $removed_soldiers = message.removed_soldiers;
  if ($removed_soldiers !== undefined) {
    writeVarint32(bb, 34);
    let nested = popByteBuffer();
    _encodeRemovedSoldiers($removed_soldiers, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional AllKeepUpdates keep_updates = 5;
  let $keep_updates = message.keep_updates;
  if ($keep_updates !== undefined) {
    writeVarint32(bb, 42);
    let nested = popByteBuffer();
    _encodeAllKeepUpdates($keep_updates, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional NewProjectiles new_projectiles = 6;
  let $new_projectiles = message.new_projectiles;
  if ($new_projectiles !== undefined) {
    writeVarint32(bb, 50);
    let nested = popByteBuffer();
    _encodeNewProjectiles($new_projectiles, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional RenderTileUpdates render_tile_updates = 7;
  let $render_tile_updates = message.render_tile_updates;
  if ($render_tile_updates !== undefined) {
    writeVarint32(bb, 58);
    let nested = popByteBuffer();
    _encodeRenderTileUpdates($render_tile_updates, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional NewWords new_words = 8;
  let $new_words = message.new_words;
  if ($new_words !== undefined) {
    writeVarint32(bb, 66);
    let nested = popByteBuffer();
    _encodeNewWords($new_words, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeOneof_GameServerToPlayer(binary: Uint8Array): Oneof_GameServerToPlayer {
  return _decodeOneof_GameServerToPlayer(wrapByteBuffer(binary));
}

function _decodeOneof_GameServerToPlayer(bb: ByteBuffer): Oneof_GameServerToPlayer {
  let message: Oneof_GameServerToPlayer = {} as any;

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

      // optional NewSoldiers new_soldiers = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.new_soldiers = _decodeNewSoldiers(bb);
        bb.limit = limit;
        break;
      }

      // optional RemovedSoldiers removed_soldiers = 4;
      case 4: {
        let limit = pushTemporaryLength(bb);
        message.removed_soldiers = _decodeRemovedSoldiers(bb);
        bb.limit = limit;
        break;
      }

      // optional AllKeepUpdates keep_updates = 5;
      case 5: {
        let limit = pushTemporaryLength(bb);
        message.keep_updates = _decodeAllKeepUpdates(bb);
        bb.limit = limit;
        break;
      }

      // optional NewProjectiles new_projectiles = 6;
      case 6: {
        let limit = pushTemporaryLength(bb);
        message.new_projectiles = _decodeNewProjectiles(bb);
        bb.limit = limit;
        break;
      }

      // optional RenderTileUpdates render_tile_updates = 7;
      case 7: {
        let limit = pushTemporaryLength(bb);
        message.render_tile_updates = _decodeRenderTileUpdates(bb);
        bb.limit = limit;
        break;
      }

      // optional NewWords new_words = 8;
      case 8: {
        let limit = pushTemporaryLength(bb);
        message.new_words = _decodeNewWords(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface RenderTile {
  tile_case?: number;
  corner_alliance?: number[];
  alliance_case?: RenderAllianceCase;
}

export function encodeRenderTile(message: RenderTile): Uint8Array {
  let bb = popByteBuffer();
  _encodeRenderTile(message, bb);
  return toUint8Array(bb);
}

function _encodeRenderTile(message: RenderTile, bb: ByteBuffer): void {
  // optional uint32 tile_case = 1;
  let $tile_case = message.tile_case;
  if ($tile_case !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $tile_case);
  }

  // repeated int32 corner_alliance = 2;
  let array$corner_alliance = message.corner_alliance;
  if (array$corner_alliance !== undefined) {
    let packed = popByteBuffer();
    for (let value of array$corner_alliance) {
      writeVarint64(packed, intToLong(value));
    }
    writeVarint32(bb, 18);
    writeVarint32(bb, packed.offset);
    writeByteBuffer(bb, packed);
    pushByteBuffer(packed);
  }

  // optional RenderAllianceCase alliance_case = 3;
  let $alliance_case = message.alliance_case;
  if ($alliance_case !== undefined) {
    writeVarint32(bb, 24);
    writeVarint32(bb, encodeRenderAllianceCase[$alliance_case]);
  }
}

export function decodeRenderTile(binary: Uint8Array): RenderTile {
  return _decodeRenderTile(wrapByteBuffer(binary));
}

function _decodeRenderTile(bb: ByteBuffer): RenderTile {
  let message: RenderTile = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 tile_case = 1;
      case 1: {
        message.tile_case = readVarint32(bb) >>> 0;
        break;
      }

      // repeated int32 corner_alliance = 2;
      case 2: {
        let values = message.corner_alliance || (message.corner_alliance = []);
        if ((tag & 7) === 2) {
          let outerLimit = pushTemporaryLength(bb);
          while (!isAtEnd(bb)) {
            values.push(readVarint32(bb));
          }
          bb.limit = outerLimit;
        } else {
          values.push(readVarint32(bb));
        }
        break;
      }

      // optional RenderAllianceCase alliance_case = 3;
      case 3: {
        message.alliance_case = decodeRenderAllianceCase[readVarint32(bb)];
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

export interface V3 {
  x?: number;
  y?: number;
  z?: number;
}

export function encodeV3(message: V3): Uint8Array {
  let bb = popByteBuffer();
  _encodeV3(message, bb);
  return toUint8Array(bb);
}

function _encodeV3(message: V3, bb: ByteBuffer): void {
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

  // optional float z = 3;
  let $z = message.z;
  if ($z !== undefined) {
    writeVarint32(bb, 29);
    writeFloat(bb, $z);
  }
}

export function decodeV3(binary: Uint8Array): V3 {
  return _decodeV3(wrapByteBuffer(binary));
}

function _decodeV3(bb: ByteBuffer): V3 {
  let message: V3 = {} as any;

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

      // optional float z = 3;
      case 3: {
        message.z = readFloat(bb);
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

export interface NewSoldier {
  id?: number;
  movement_speed?: number;
  type?: SoldierType;
  source_keep_id?: number;
  target_keep_id?: number;
  row_offset?: number;
}

export function encodeNewSoldier(message: NewSoldier): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewSoldier(message, bb);
  return toUint8Array(bb);
}

function _encodeNewSoldier(message: NewSoldier, bb: ByteBuffer): void {
  // optional uint32 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $id);
  }

  // optional float movement_speed = 2;
  let $movement_speed = message.movement_speed;
  if ($movement_speed !== undefined) {
    writeVarint32(bb, 21);
    writeFloat(bb, $movement_speed);
  }

  // optional SoldierType type = 3;
  let $type = message.type;
  if ($type !== undefined) {
    writeVarint32(bb, 24);
    writeVarint32(bb, encodeSoldierType[$type]);
  }

  // optional uint32 source_keep_id = 4;
  let $source_keep_id = message.source_keep_id;
  if ($source_keep_id !== undefined) {
    writeVarint32(bb, 32);
    writeVarint32(bb, $source_keep_id);
  }

  // optional uint32 target_keep_id = 5;
  let $target_keep_id = message.target_keep_id;
  if ($target_keep_id !== undefined) {
    writeVarint32(bb, 40);
    writeVarint32(bb, $target_keep_id);
  }

  // optional float row_offset = 6;
  let $row_offset = message.row_offset;
  if ($row_offset !== undefined) {
    writeVarint32(bb, 53);
    writeFloat(bb, $row_offset);
  }
}

export function decodeNewSoldier(binary: Uint8Array): NewSoldier {
  return _decodeNewSoldier(wrapByteBuffer(binary));
}

function _decodeNewSoldier(bb: ByteBuffer): NewSoldier {
  let message: NewSoldier = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 id = 1;
      case 1: {
        message.id = readVarint32(bb) >>> 0;
        break;
      }

      // optional float movement_speed = 2;
      case 2: {
        message.movement_speed = readFloat(bb);
        break;
      }

      // optional SoldierType type = 3;
      case 3: {
        message.type = decodeSoldierType[readVarint32(bb)];
        break;
      }

      // optional uint32 source_keep_id = 4;
      case 4: {
        message.source_keep_id = readVarint32(bb) >>> 0;
        break;
      }

      // optional uint32 target_keep_id = 5;
      case 5: {
        message.target_keep_id = readVarint32(bb) >>> 0;
        break;
      }

      // optional float row_offset = 6;
      case 6: {
        message.row_offset = readFloat(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface NewSoldiers {
  soldiers?: NewSoldier[];
}

export function encodeNewSoldiers(message: NewSoldiers): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewSoldiers(message, bb);
  return toUint8Array(bb);
}

function _encodeNewSoldiers(message: NewSoldiers, bb: ByteBuffer): void {
  // repeated NewSoldier soldiers = 1;
  let array$soldiers = message.soldiers;
  if (array$soldiers !== undefined) {
    for (let value of array$soldiers) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeNewSoldier(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeNewSoldiers(binary: Uint8Array): NewSoldiers {
  return _decodeNewSoldiers(wrapByteBuffer(binary));
}

function _decodeNewSoldiers(bb: ByteBuffer): NewSoldiers {
  let message: NewSoldiers = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated NewSoldier soldiers = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.soldiers || (message.soldiers = []);
        values.push(_decodeNewSoldier(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface RemovedSoldiers {
  soldier_ids?: number[];
}

export function encodeRemovedSoldiers(message: RemovedSoldiers): Uint8Array {
  let bb = popByteBuffer();
  _encodeRemovedSoldiers(message, bb);
  return toUint8Array(bb);
}

function _encodeRemovedSoldiers(message: RemovedSoldiers, bb: ByteBuffer): void {
  // repeated uint32 soldier_ids = 1;
  let array$soldier_ids = message.soldier_ids;
  if (array$soldier_ids !== undefined) {
    let packed = popByteBuffer();
    for (let value of array$soldier_ids) {
      writeVarint32(packed, value);
    }
    writeVarint32(bb, 10);
    writeVarint32(bb, packed.offset);
    writeByteBuffer(bb, packed);
    pushByteBuffer(packed);
  }
}

export function decodeRemovedSoldiers(binary: Uint8Array): RemovedSoldiers {
  return _decodeRemovedSoldiers(wrapByteBuffer(binary));
}

function _decodeRemovedSoldiers(bb: ByteBuffer): RemovedSoldiers {
  let message: RemovedSoldiers = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated uint32 soldier_ids = 1;
      case 1: {
        let values = message.soldier_ids || (message.soldier_ids = []);
        if ((tag & 7) === 2) {
          let outerLimit = pushTemporaryLength(bb);
          while (!isAtEnd(bb)) {
            values.push(readVarint32(bb) >>> 0);
          }
          bb.limit = outerLimit;
        } else {
          values.push(readVarint32(bb) >>> 0);
        }
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface NewWord {
  grid_pos?: V2Int;
  text?: string;
  owning_keep_pos?: V2;
}

export function encodeNewWord(message: NewWord): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewWord(message, bb);
  return toUint8Array(bb);
}

function _encodeNewWord(message: NewWord, bb: ByteBuffer): void {
  // optional V2Int grid_pos = 1;
  let $grid_pos = message.grid_pos;
  if ($grid_pos !== undefined) {
    writeVarint32(bb, 10);
    let nested = popByteBuffer();
    _encodeV2Int($grid_pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional string text = 2;
  let $text = message.text;
  if ($text !== undefined) {
    writeVarint32(bb, 18);
    writeString(bb, $text);
  }

  // optional V2 owning_keep_pos = 3;
  let $owning_keep_pos = message.owning_keep_pos;
  if ($owning_keep_pos !== undefined) {
    writeVarint32(bb, 26);
    let nested = popByteBuffer();
    _encodeV2($owning_keep_pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeNewWord(binary: Uint8Array): NewWord {
  return _decodeNewWord(wrapByteBuffer(binary));
}

function _decodeNewWord(bb: ByteBuffer): NewWord {
  let message: NewWord = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional V2Int grid_pos = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        message.grid_pos = _decodeV2Int(bb);
        bb.limit = limit;
        break;
      }

      // optional string text = 2;
      case 2: {
        message.text = readString(bb, readVarint32(bb));
        break;
      }

      // optional V2 owning_keep_pos = 3;
      case 3: {
        let limit = pushTemporaryLength(bb);
        message.owning_keep_pos = _decodeV2(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface NewWords {
  words?: NewWord[];
}

export function encodeNewWords(message: NewWords): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewWords(message, bb);
  return toUint8Array(bb);
}

function _encodeNewWords(message: NewWords, bb: ByteBuffer): void {
  // repeated NewWord words = 1;
  let array$words = message.words;
  if (array$words !== undefined) {
    for (let value of array$words) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeNewWord(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeNewWords(binary: Uint8Array): NewWords {
  return _decodeNewWords(wrapByteBuffer(binary));
}

function _decodeNewWords(bb: ByteBuffer): NewWords {
  let message: NewWords = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated NewWord words = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.words || (message.words = []);
        values.push(_decodeNewWord(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface PathToKeep {
  target_id?: number;
  path?: V2[];
  walk_types?: WalkPathType[];
}

export function encodePathToKeep(message: PathToKeep): Uint8Array {
  let bb = popByteBuffer();
  _encodePathToKeep(message, bb);
  return toUint8Array(bb);
}

function _encodePathToKeep(message: PathToKeep, bb: ByteBuffer): void {
  // optional uint32 target_id = 1;
  let $target_id = message.target_id;
  if ($target_id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $target_id);
  }

  // repeated V2 path = 2;
  let array$path = message.path;
  if (array$path !== undefined) {
    for (let value of array$path) {
      writeVarint32(bb, 18);
      let nested = popByteBuffer();
      _encodeV2(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }

  // repeated WalkPathType walk_types = 3;
  let array$walk_types = message.walk_types;
  if (array$walk_types !== undefined) {
    let packed = popByteBuffer();
    for (let value of array$walk_types) {
      writeVarint32(packed, encodeWalkPathType[value]);
    }
    writeVarint32(bb, 26);
    writeVarint32(bb, packed.offset);
    writeByteBuffer(bb, packed);
    pushByteBuffer(packed);
  }
}

export function decodePathToKeep(binary: Uint8Array): PathToKeep {
  return _decodePathToKeep(wrapByteBuffer(binary));
}

function _decodePathToKeep(bb: ByteBuffer): PathToKeep {
  let message: PathToKeep = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 target_id = 1;
      case 1: {
        message.target_id = readVarint32(bb) >>> 0;
        break;
      }

      // repeated V2 path = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        let values = message.path || (message.path = []);
        values.push(_decodeV2(bb));
        bb.limit = limit;
        break;
      }

      // repeated WalkPathType walk_types = 3;
      case 3: {
        let values = message.walk_types || (message.walk_types = []);
        if ((tag & 7) === 2) {
          let outerLimit = pushTemporaryLength(bb);
          while (!isAtEnd(bb)) {
            values.push(decodeWalkPathType[readVarint32(bb)]);
          }
          bb.limit = outerLimit;
        } else {
          values.push(decodeWalkPathType[readVarint32(bb)]);
        }
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface KeepState {
  id?: number;
  pos?: V2;
  warrior_count?: number;
  archer_count?: number;
  name?: string;
  alliance?: number;
  paths?: PathToKeep[];
}

export function encodeKeepState(message: KeepState): Uint8Array {
  let bb = popByteBuffer();
  _encodeKeepState(message, bb);
  return toUint8Array(bb);
}

function _encodeKeepState(message: KeepState, bb: ByteBuffer): void {
  // optional uint32 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $id);
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

  // repeated PathToKeep paths = 7;
  let array$paths = message.paths;
  if (array$paths !== undefined) {
    for (let value of array$paths) {
      writeVarint32(bb, 58);
      let nested = popByteBuffer();
      _encodePathToKeep(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
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

      // optional uint32 id = 1;
      case 1: {
        message.id = readVarint32(bb) >>> 0;
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

      // repeated PathToKeep paths = 7;
      case 7: {
        let limit = pushTemporaryLength(bb);
        let values = message.paths || (message.paths = []);
        values.push(_decodePathToKeep(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface KeepUpdate {
  id?: number;
  warrior_count?: number;
  archer_count?: number;
  alliance?: number;
}

export function encodeKeepUpdate(message: KeepUpdate): Uint8Array {
  let bb = popByteBuffer();
  _encodeKeepUpdate(message, bb);
  return toUint8Array(bb);
}

function _encodeKeepUpdate(message: KeepUpdate, bb: ByteBuffer): void {
  // optional uint32 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $id);
  }

  // optional int32 warrior_count = 2;
  let $warrior_count = message.warrior_count;
  if ($warrior_count !== undefined) {
    writeVarint32(bb, 16);
    writeVarint64(bb, intToLong($warrior_count));
  }

  // optional int32 archer_count = 3;
  let $archer_count = message.archer_count;
  if ($archer_count !== undefined) {
    writeVarint32(bb, 24);
    writeVarint64(bb, intToLong($archer_count));
  }

  // optional int32 alliance = 4;
  let $alliance = message.alliance;
  if ($alliance !== undefined) {
    writeVarint32(bb, 32);
    writeVarint64(bb, intToLong($alliance));
  }
}

export function decodeKeepUpdate(binary: Uint8Array): KeepUpdate {
  return _decodeKeepUpdate(wrapByteBuffer(binary));
}

function _decodeKeepUpdate(bb: ByteBuffer): KeepUpdate {
  let message: KeepUpdate = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 id = 1;
      case 1: {
        message.id = readVarint32(bb) >>> 0;
        break;
      }

      // optional int32 warrior_count = 2;
      case 2: {
        message.warrior_count = readVarint32(bb);
        break;
      }

      // optional int32 archer_count = 3;
      case 3: {
        message.archer_count = readVarint32(bb);
        break;
      }

      // optional int32 alliance = 4;
      case 4: {
        message.alliance = readVarint32(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface AllKeepUpdates {
  keep_updates?: KeepUpdate[];
}

export function encodeAllKeepUpdates(message: AllKeepUpdates): Uint8Array {
  let bb = popByteBuffer();
  _encodeAllKeepUpdates(message, bb);
  return toUint8Array(bb);
}

function _encodeAllKeepUpdates(message: AllKeepUpdates, bb: ByteBuffer): void {
  // repeated KeepUpdate keep_updates = 1;
  let array$keep_updates = message.keep_updates;
  if (array$keep_updates !== undefined) {
    for (let value of array$keep_updates) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeKeepUpdate(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeAllKeepUpdates(binary: Uint8Array): AllKeepUpdates {
  return _decodeAllKeepUpdates(wrapByteBuffer(binary));
}

function _decodeAllKeepUpdates(bb: ByteBuffer): AllKeepUpdates {
  let message: AllKeepUpdates = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated KeepUpdate keep_updates = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.keep_updates || (message.keep_updates = []);
        values.push(_decodeKeepUpdate(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface NewProjectile {
  id?: number;
  start_pos?: V3;
  birth_time?: number;
  initial_velocity?: V3;
  time_will_land?: number;
  gravitational_force?: number;
}

export function encodeNewProjectile(message: NewProjectile): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewProjectile(message, bb);
  return toUint8Array(bb);
}

function _encodeNewProjectile(message: NewProjectile, bb: ByteBuffer): void {
  // optional uint32 id = 1;
  let $id = message.id;
  if ($id !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $id);
  }

  // optional V3 start_pos = 2;
  let $start_pos = message.start_pos;
  if ($start_pos !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeV3($start_pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional float birth_time = 3;
  let $birth_time = message.birth_time;
  if ($birth_time !== undefined) {
    writeVarint32(bb, 29);
    writeFloat(bb, $birth_time);
  }

  // optional V3 initial_velocity = 4;
  let $initial_velocity = message.initial_velocity;
  if ($initial_velocity !== undefined) {
    writeVarint32(bb, 34);
    let nested = popByteBuffer();
    _encodeV3($initial_velocity, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional float time_will_land = 5;
  let $time_will_land = message.time_will_land;
  if ($time_will_land !== undefined) {
    writeVarint32(bb, 45);
    writeFloat(bb, $time_will_land);
  }

  // optional float gravitational_force = 6;
  let $gravitational_force = message.gravitational_force;
  if ($gravitational_force !== undefined) {
    writeVarint32(bb, 53);
    writeFloat(bb, $gravitational_force);
  }
}

export function decodeNewProjectile(binary: Uint8Array): NewProjectile {
  return _decodeNewProjectile(wrapByteBuffer(binary));
}

function _decodeNewProjectile(bb: ByteBuffer): NewProjectile {
  let message: NewProjectile = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 id = 1;
      case 1: {
        message.id = readVarint32(bb) >>> 0;
        break;
      }

      // optional V3 start_pos = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.start_pos = _decodeV3(bb);
        bb.limit = limit;
        break;
      }

      // optional float birth_time = 3;
      case 3: {
        message.birth_time = readFloat(bb);
        break;
      }

      // optional V3 initial_velocity = 4;
      case 4: {
        let limit = pushTemporaryLength(bb);
        message.initial_velocity = _decodeV3(bb);
        bb.limit = limit;
        break;
      }

      // optional float time_will_land = 5;
      case 5: {
        message.time_will_land = readFloat(bb);
        break;
      }

      // optional float gravitational_force = 6;
      case 6: {
        message.gravitational_force = readFloat(bb);
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface NewProjectiles {
  projectiles?: NewProjectile[];
}

export function encodeNewProjectiles(message: NewProjectiles): Uint8Array {
  let bb = popByteBuffer();
  _encodeNewProjectiles(message, bb);
  return toUint8Array(bb);
}

function _encodeNewProjectiles(message: NewProjectiles, bb: ByteBuffer): void {
  // repeated NewProjectile projectiles = 1;
  let array$projectiles = message.projectiles;
  if (array$projectiles !== undefined) {
    for (let value of array$projectiles) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeNewProjectile(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeNewProjectiles(binary: Uint8Array): NewProjectiles {
  return _decodeNewProjectiles(wrapByteBuffer(binary));
}

function _decodeNewProjectiles(bb: ByteBuffer): NewProjectiles {
  let message: NewProjectiles = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated NewProjectile projectiles = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.projectiles || (message.projectiles = []);
        values.push(_decodeNewProjectile(bb));
        bb.limit = limit;
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
  render_tiles?: RenderTile[];
  words?: NewWord[];
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

  // repeated RenderTile render_tiles = 5;
  let array$render_tiles = message.render_tiles;
  if (array$render_tiles !== undefined) {
    for (let value of array$render_tiles) {
      writeVarint32(bb, 42);
      let nested = popByteBuffer();
      _encodeRenderTile(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }

  // repeated NewWord words = 6;
  let array$words = message.words;
  if (array$words !== undefined) {
    for (let value of array$words) {
      writeVarint32(bb, 50);
      let nested = popByteBuffer();
      _encodeNewWord(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
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

      // repeated RenderTile render_tiles = 5;
      case 5: {
        let limit = pushTemporaryLength(bb);
        let values = message.render_tiles || (message.render_tiles = []);
        values.push(_decodeRenderTile(bb));
        bb.limit = limit;
        break;
      }

      // repeated NewWord words = 6;
      case 6: {
        let limit = pushTemporaryLength(bb);
        let values = message.words || (message.words = []);
        values.push(_decodeNewWord(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface RenderTileUpdate {
  pos?: V2;
  render_tile?: RenderTile;
}

export function encodeRenderTileUpdate(message: RenderTileUpdate): Uint8Array {
  let bb = popByteBuffer();
  _encodeRenderTileUpdate(message, bb);
  return toUint8Array(bb);
}

function _encodeRenderTileUpdate(message: RenderTileUpdate, bb: ByteBuffer): void {
  // optional V2 pos = 1;
  let $pos = message.pos;
  if ($pos !== undefined) {
    writeVarint32(bb, 10);
    let nested = popByteBuffer();
    _encodeV2($pos, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }

  // optional RenderTile render_tile = 2;
  let $render_tile = message.render_tile;
  if ($render_tile !== undefined) {
    writeVarint32(bb, 18);
    let nested = popByteBuffer();
    _encodeRenderTile($render_tile, nested);
    writeVarint32(bb, nested.limit);
    writeByteBuffer(bb, nested);
    pushByteBuffer(nested);
  }
}

export function decodeRenderTileUpdate(binary: Uint8Array): RenderTileUpdate {
  return _decodeRenderTileUpdate(wrapByteBuffer(binary));
}

function _decodeRenderTileUpdate(bb: ByteBuffer): RenderTileUpdate {
  let message: RenderTileUpdate = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional V2 pos = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        message.pos = _decodeV2(bb);
        bb.limit = limit;
        break;
      }

      // optional RenderTile render_tile = 2;
      case 2: {
        let limit = pushTemporaryLength(bb);
        message.render_tile = _decodeRenderTile(bb);
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface RenderTileUpdates {
  render_tile_updates?: RenderTileUpdate[];
}

export function encodeRenderTileUpdates(message: RenderTileUpdates): Uint8Array {
  let bb = popByteBuffer();
  _encodeRenderTileUpdates(message, bb);
  return toUint8Array(bb);
}

function _encodeRenderTileUpdates(message: RenderTileUpdates, bb: ByteBuffer): void {
  // repeated RenderTileUpdate render_tile_updates = 1;
  let array$render_tile_updates = message.render_tile_updates;
  if (array$render_tile_updates !== undefined) {
    for (let value of array$render_tile_updates) {
      writeVarint32(bb, 10);
      let nested = popByteBuffer();
      _encodeRenderTileUpdate(value, nested);
      writeVarint32(bb, nested.limit);
      writeByteBuffer(bb, nested);
      pushByteBuffer(nested);
    }
  }
}

export function decodeRenderTileUpdates(binary: Uint8Array): RenderTileUpdates {
  return _decodeRenderTileUpdates(wrapByteBuffer(binary));
}

function _decodeRenderTileUpdates(bb: ByteBuffer): RenderTileUpdates {
  let message: RenderTileUpdates = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // repeated RenderTileUpdate render_tile_updates = 1;
      case 1: {
        let limit = pushTemporaryLength(bb);
        let values = message.render_tile_updates || (message.render_tile_updates = []);
        values.push(_decodeRenderTileUpdate(bb));
        bb.limit = limit;
        break;
      }

      default:
        skipUnknownField(bb, tag & 7);
    }
  }

  return message;
}

export interface IssueDeploymentOrder {
  source_keep?: number;
  target_keep?: number;
  soldier_type?: SoldierType;
  percent?: number;
}

export function encodeIssueDeploymentOrder(message: IssueDeploymentOrder): Uint8Array {
  let bb = popByteBuffer();
  _encodeIssueDeploymentOrder(message, bb);
  return toUint8Array(bb);
}

function _encodeIssueDeploymentOrder(message: IssueDeploymentOrder, bb: ByteBuffer): void {
  // optional uint32 source_keep = 1;
  let $source_keep = message.source_keep;
  if ($source_keep !== undefined) {
    writeVarint32(bb, 8);
    writeVarint32(bb, $source_keep);
  }

  // optional uint32 target_keep = 2;
  let $target_keep = message.target_keep;
  if ($target_keep !== undefined) {
    writeVarint32(bb, 16);
    writeVarint32(bb, $target_keep);
  }

  // optional SoldierType soldier_type = 3;
  let $soldier_type = message.soldier_type;
  if ($soldier_type !== undefined) {
    writeVarint32(bb, 24);
    writeVarint32(bb, encodeSoldierType[$soldier_type]);
  }

  // optional float percent = 4;
  let $percent = message.percent;
  if ($percent !== undefined) {
    writeVarint32(bb, 37);
    writeFloat(bb, $percent);
  }
}

export function decodeIssueDeploymentOrder(binary: Uint8Array): IssueDeploymentOrder {
  return _decodeIssueDeploymentOrder(wrapByteBuffer(binary));
}

function _decodeIssueDeploymentOrder(bb: ByteBuffer): IssueDeploymentOrder {
  let message: IssueDeploymentOrder = {} as any;

  end_of_message: while (!isAtEnd(bb)) {
    let tag = readVarint32(bb);

    switch (tag >>> 3) {
      case 0:
        break end_of_message;

      // optional uint32 source_keep = 1;
      case 1: {
        message.source_keep = readVarint32(bb) >>> 0;
        break;
      }

      // optional uint32 target_keep = 2;
      case 2: {
        message.target_keep = readVarint32(bb) >>> 0;
        break;
      }

      // optional SoldierType soldier_type = 3;
      case 3: {
        message.soldier_type = decodeSoldierType[readVarint32(bb)];
        break;
      }

      // optional float percent = 4;
      case 4: {
        message.percent = readFloat(bb);
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
