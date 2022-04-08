const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  lintOnSave: false,
  pwa: {
    themeColor: '#17a2b8',
    msTileColor: '#000000',
    iconPaths: {
      msTileImage: 'img/icons/mstile-150x150.png'
    }
  }
})
