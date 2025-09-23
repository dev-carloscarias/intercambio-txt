import React from 'react';
import { shallow } from 'enzyme';
import FilterBar from './FilterBar';

describe('FilterBar component', () => {
    it('should render the component', () => {
        const props = {
            isOpen: false,
            onApply: () => {},
            disabled: false
        };
        const wrapper = shallow(<FilterBar {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
