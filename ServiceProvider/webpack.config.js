const webpack = require('webpack');
require('dotenv').config();
const path = require('path');
const { merge } = require('webpack-merge');
const singleSpaDefaults = require('webpack-config-single-spa-react-ts');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const { HOST: host, PORT: port, NODE_ENV: mode } = process.env;

const isDev = !process.env.NODE_ENV || process.env.NODE_ENV === 'development';

module.exports = (webpackConfigEnv, argv) => {
    const defaultConfig = singleSpaDefaults({
        orgName: 'provider-portal',
        projectName: 'clinical-consultation-create-servicing-provider',
        webpackConfigEnv,
        argv
    });

    return merge(defaultConfig, {
        devServer: {
            host,
            port
        },
        output: {
            path: path.resolve(__dirname, process.env.distPath)
        },
        module: {
            rules: [
                {
                    test: /\.(png|jpe?g|gif|svg)$/i,
                    use: [
                        {
                            loader: 'file-loader'
                        }
                    ]
                },
                {
                    test: /\.s[ac]ss$/i,
                    exclude: /\.module.(s(a|c)ss)$/,
                    use: [
                        isDev ? 'style-loader' : MiniCssExtractPlugin.loader,
                        'css-loader',
                        'postcss-loader',
                        // Compiles Sass to CSS
                        {
                            loader: 'sass-loader',
                            options: {
                                sassOptions: {
                                    includePaths: [
                                        'node_modules/@provider-portal/styles/styles/'
                                    ]
                                }
                            }
                        }
                    ]
                },
                {
                    test: /\.module\.s[ac]ss$/i,
                    use: [
                        isDev ? 'style-loader' : MiniCssExtractPlugin.loader,
                        {
                            loader: 'css-loader',
                            options: {
                                modules: {
                                    exportLocalsConvention: 'camelCase'
                                }
                            }
                        },
                        'postcss-loader',
                        // Compiles Sass to CSS
                        {
                            loader: 'sass-loader',
                            options: {
                                sassOptions: {
                                    includePaths: [
                                        'node_modules/@provider-portal/styles/styles/'
                                    ]
                                },
                                sourceMap: isDev
                            }
                        }
                    ]
                }
            ]
        },
        resolve: {
            extensions: ['.ts', '.tsx', '.js', '.css', '.scss'],
            symlinks: false
        },
        plugins: [
            ///...
            new MiniCssExtractPlugin({
                filename: isDev ? '[name].css' : '[name].[hash].css',
                chunkFilename: isDev ? '[id].css' : '[id].[hash].css'
            }),
            new webpack.DefinePlugin({
                'process.env.API_URL': JSON.stringify(process.env.apiUrl)
            })
        ],
        externals: [
            'reactstrap',
            'react-i18next',
            'i18next',
            'axios',
            'single-spa'
        ]
    });
};
