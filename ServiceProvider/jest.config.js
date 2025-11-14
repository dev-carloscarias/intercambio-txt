module.exports = {
    rootDir: 'src',
    transform: {
        '^.+\\.jsx?$': 'babel-jest',
        '^.+\\.tsx?$': 'ts-jest'
    },
    moduleNameMapper: {
        'single-spa-react/parcel': 'single-spa-react/lib/cjs/parcel.cjs',
        '\\.(css|scss)$': '<rootDir>/__mocks__/fileMock.js',
        '\\.(jpg|jpeg|png|gif|eot|otf|webp|svg|ttf|woff|woff2|mp4|webm|wav|mp3|m4a|aac|oga)$':
            '<rootDir>/__mocks__/fileMock.js'
    },
    setupFilesAfterEnv: ['<rootDir>setupTests.js']
};
