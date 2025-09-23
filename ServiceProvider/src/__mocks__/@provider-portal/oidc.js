export const UserService = {
    getInstance: () => {
        return {
            isAuthenticated: () => true,
            getActiveRole: () => {
                return { name: 'PCP', userRoleId: 1 };
            },
            getUser: () => {
                return { fullName: 'Juan del Pueblo' };
            },
            getAccessToken: () => ''
        };
    }
};
