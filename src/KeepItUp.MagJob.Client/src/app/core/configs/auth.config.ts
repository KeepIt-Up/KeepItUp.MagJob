import { AuthConfig } from "angular-oauth2-oidc";
import { environment } from "../../../environments/environment";

export const authCodeFlowConfig: AuthConfig = environment.keycloakConfig;

