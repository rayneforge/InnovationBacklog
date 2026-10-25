import type { AppUser } from "@momentum/contracts";
import type { Assert, FieldsExistOn } from "./common.js";
import type { Role } from "./enums.js";

export interface CurrentUser {
  id: string;
  sub: string;
  email: string;
  displayName: string;
  createdAt: string;
  role: Role;
}

/** Guards the fields that do come from the wire. `role` is provider-derived. */
export type CurrentUserMatchesWire = Assert<FieldsExistOn<Omit<CurrentUser, "role">, AppUser>>;

/** A person referenced from an engagement record, for avatars and "shared by". */
export interface UserRef {
  id: string;
  displayName?: string;
  email?: string;
}
