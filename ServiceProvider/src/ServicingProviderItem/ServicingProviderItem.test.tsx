import React from 'react';
import { shallow } from 'enzyme';
import ServicingProviderItem from './ServicingProviderItem';

describe('ServicingProviderItem component', () => {
    it('should render the component', () => {
        const props = {
            item: {},
            onCheck: i => {},
            checked: false,
            autoSelectCity: false
        };
        const wrapper = shallow(<ServicingProviderItem {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
