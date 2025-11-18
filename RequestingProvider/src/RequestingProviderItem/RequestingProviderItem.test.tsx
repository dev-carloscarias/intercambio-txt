import React from 'react';
import { shallow } from 'enzyme';
import RequestingProviderItem from './RequestingProviderItem';

describe('RequestingProviderItem component', () => {
    it('should render the component', () => {
        const props = {
            item: {
                npi: '123456',
                name: 'John Doe',
                phoneNumber: '1234',
                email: 'mail@mail.com',
                specialties: [],
                autoSelectCity: false,
                id: 22,
                renderingProviderId: 1234,
                renderingProviderNPI: 'string',
                renderingProviderName: 'string'
            },
            onCheck: i => {},
            checked: false
        };
        const wrapper = shallow(<RequestingProviderItem {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
