import React from 'react';
import { shallow } from 'enzyme';
import FilterPanel from './FilterPanel';

describe('FilterPanel component', () => {
    it('should render the component', () => {
        const props = {
            isOpen: false,
            onClose: () => {},
            searchTerm: 'Term',
            onApply: (filters, filterCount) => {},
            totalResults: 100
        };
        const wrapper = shallow(<FilterPanel {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
