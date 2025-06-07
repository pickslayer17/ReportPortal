import { jwtDecode } from "jwt-decode";

export function getUserRoleFromToken(): string | null {
    const token = document.cookie.split('; ').find(row => row.startsWith('token='))?.split('=')[1];
    if (!token) return null;
    try {
        const decoded: any = jwtDecode(token);
        return (
            decoded.role ||
            decoded.roles ||
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
            null
        );
    } catch {
        return null;
    }
}