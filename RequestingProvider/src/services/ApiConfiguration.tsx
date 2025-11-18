import axios from 'axios';
import { getRequestLanguage } from '@provider-portal/i18n';
import { UserService } from '@provider-portal/oidc';

const instance = axios.create({
    baseURL: process.env.API_URL,
    headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
    },
    responseType: 'json'
});

instance.interceptors.request.use(
    async config => {
        config.headers.Authorization = `Bearer ${UserService.getInstance().getAccessToken()}`;
        config.headers['Accept-Language'] = getRequestLanguage();
        return config;
    },
    async error => {
        return Promise.reject(error);
    }
);

export default instance;
