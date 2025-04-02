import { JwtPayload } from 'jwt-decode';

export const NameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
export const UserIdClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
export const RoleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

export interface CustomJwtPayload extends JwtPayload {
    // [key: string]: string | string[] | number | undefined;
    [NameClaim]: string;
    [UserIdClaim]: string;
    [RoleClaim]: string;
}