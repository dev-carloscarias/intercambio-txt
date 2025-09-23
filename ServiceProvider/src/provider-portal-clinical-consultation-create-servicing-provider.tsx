import './set-public-path';
import React from 'react';
import ReactDOM from 'react-dom';
import singleSpaReact from 'single-spa-react';
import App from './App';

const lifecycles = singleSpaReact({
    React,
    ReactDOM,
    rootComponent: App,
    domElementGetter: () => {
        let contentWrapper = document.getElementById('content-wrapper');
        let el = document.getElementById(
            'smartprofile-referral-create-servicing-provider'
        );

        if (contentWrapper && !el) {
            el = document.createElement('div');
            el.id = 'smartprofile-referral-create-servicing-provider';
            contentWrapper.appendChild(el);
        }
        return el || document.body;
    }
});

export const bootstrap = lifecycles.bootstrap;
export const mount = lifecycles.mount;
export const unmount = lifecycles.unmount;
export const update = lifecycles.update;
