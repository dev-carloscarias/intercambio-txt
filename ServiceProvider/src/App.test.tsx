import React from 'react';
import { shallow } from 'enzyme';
import App from './App';

describe('App component', () => {
    it('should create a component', () => {
        const props = {
            onValidate: () => {},
            consultation: null,
            active: false
        };
        const container = shallow(<App {...props} />);
        expect(container).toBeTruthy();
    });
});
