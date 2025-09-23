import React from 'react';
import { shallow } from 'enzyme';
import ServicingProviderSelectedItem from './ServicingProviderSelectedItem';

describe('ServicingProviderSelectedItem component', () => {
    it('should render the component', () => {
        const props = {
            item: {
                renderingProviderId: 1,
                renderingProviderNPI: '123456',
                renderingProviderName: 'John Doe',
                phoneNumber: '1234',
                email: 'mail@mail.com',
                specialties: [],
                city: []
            },
            onSelectCity: () => {},
            id: 'id'
        };
        const wrapper = shallow(<ServicingProviderSelectedItem {...props} />);
        expect(wrapper).toBeTruthy();
    });
});
