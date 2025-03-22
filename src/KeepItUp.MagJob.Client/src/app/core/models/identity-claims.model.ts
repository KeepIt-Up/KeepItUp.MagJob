export interface IdentityClaims {
  sub: string;
  given_name?: string;
  family_name?: string;
  email?: string;
  [key: string]: unknown;
}
