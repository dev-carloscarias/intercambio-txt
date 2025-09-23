declare module '*.scss' {
    export const content: { [className: string]: string };
    export default content;
}

declare module '*.png';
declare module '*.jpg';
declare module '*.svg';

declare module '@provider-portal/i18n';
declare module '@provider-portal/icons';
declare module '@provider-portal/components';
declare module '@provider-portal/oidc';
