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
            getAccessToken: () => '',
            getSessionDuration: () => 1200,
            hasFeature: () => true
        };
    }
};
export const AuthorizationService = {
    getInstance: () => {
        return {
            signout: () => Promise.resolve(),
            lock: () => Promise.resolve()
        };
    }
};
